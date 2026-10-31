using Assets.Project.Code.Scripts.Map;
using Assets.Project.Code.Scripts.Map.Help;
using Assets.Project.Code.Standard.Collections;
using Assets.Project.Code.Standard.TimeHandlers;
using MapGenearionLibrary.Base;
using MapGenearionLibrary.Navigation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Assets.Project.Code.Scripts.Agents.Help.AgentNavigationPathHandler;

namespace Assets.Project.Code.Scripts.Agents.Help
{
    public class AgentNavigationPathHandler : IDisposable
    {
        public AgentBaseScript BaseScript { get; }
        public AgentManagerScript AgentsManagerScript { get; }
        public MapGenerationScript MapGenerationScript { get; }

        public float DistanceToObstacle { get; set; }
        public Vector3 CurrentLocation { get; private set; }
        public MapPoint CurrentMapPoint { get; private set; }
        public Vector3 TargetLocation { get; private set; }
        public MapPoint TargetMapPoint { get; private set; }
        public Vector3 CurrentTargetLocation { get; private set; }
        public MapPoint CurrentTargetMapPoint { get; private set; }

        public bool IsCollisionDetected { get; private set; } = false;
        public bool IsMovementBlocked => _PathHandler is null || _PathHandler.IsMovementBlocked;
        public bool IsNoPath => _PathHandler is null;

        private PathHandler _PathHandler { get; set; } = null;
        private MapObstacleObjectAssociation _MapLocationAssociation { get; set; }

        public AgentNavigationPathHandler(AgentManagerScript agentsManager, MapGenerationScript mapGenerationScript, AgentBaseScript baseScript)
        {
            BaseScript = baseScript;
            AgentsManagerScript = agentsManager;
            MapGenerationScript = mapGenerationScript;
            _MapLocationAssociation = new MapObstacleObjectAssociation(mapGenerationScript, baseScript);
        }

        public void SetCurrentLocation(Vector3 currentLocation)
        {
            CurrentLocation = currentLocation;
            CurrentMapPoint = MapGenerationScript.ObjectLocations.GetCellMapPoint(CurrentLocation);

            _MapLocationAssociation.Location = CurrentMapPoint;
        }

        public void SetTartgetMapPoint(MapPoint targetMapPoint)
        {
            TargetLocation = MapGenerationScript.ObjectLocations.GetCellCentralLocation(targetMapPoint);
            TargetMapPoint = targetMapPoint;

            _CreateNewPath();
        }

        public void SetTargetLocation(Vector3 targetLocation)
        {
            TargetLocation = targetLocation;
            TargetMapPoint = MapGenerationScript.ObjectLocations.GetCellMapPoint(targetLocation);

            _CreateNewPath();
        }

        public void InvokeNavigation()
        {
            IsCollisionDetected = DistanceToObstacle < AgentsManagerScript.Params.PathCollisionDetectionDistance;

            bool isTargetMapPointAchived = false;

            if (_PathHandler is not null)
            {
                isTargetMapPointAchived = _PathHandler.Invoke(
                    CurrentMapPoint
                    ,
                    IsCurrentTargetLocationAchived()
                    ,
                    IsTargetLocationAchived()
                    ,
                    IsCollisionDetected);
            }

            if (_PathHandler is not null)
            {
                CurrentTargetMapPoint = _PathHandler.NextPoint;

                if (isTargetMapPointAchived)
                {
                    CurrentTargetLocation = TargetLocation;
                }
                else
                {
                    CurrentTargetLocation = MapGenerationScript.ObjectLocations.GetCellOffsetLocation(CurrentTargetMapPoint, _PathHandler.NextPointCorrection, AgentsManagerScript.Params.CollisionResolutionOffset);
                }
            }
            else
            {
                CurrentTargetMapPoint = CurrentMapPoint;
                CurrentTargetLocation = MapGenerationScript.ObjectLocations.GetCellCentralLocation(CurrentMapPoint);
                TargetMapPoint = CurrentTargetMapPoint;
                TargetLocation = CurrentTargetLocation;
            }
        }

        public bool IsTargetLocationAchived() => MapObjectLocationHandler.CompareTwoLocations(CurrentLocation, TargetLocation, AgentsManagerScript.Params.PathTargetPointAchiveDistance);

        public bool IsCurrentTargetLocationAchived() => MapObjectLocationHandler.CompareTwoLocations(CurrentLocation, CurrentTargetLocation, AgentsManagerScript.Params.PathInternalPointAchiveDistance);

        private void _CreateNewPath()
        {
            var newPath = MapGenerationScript.MapNavigation.NavigationGraph.FindPathWave(CurrentMapPoint, TargetMapPoint, AgentsManagerScript.Params.PathSearchingDepth);
            //MapGenerationScript.MapNavigation.NavigationGraph.GetPathes(CurrentMapPoint, TargetMapPoint, AgentsManagerScript.Params.PathSearchingDepth).FirstOrDefault();

            if (_PathHandler is not null)
            {
                _PathHandler.Dispose();
            }

            if (newPath is not null)
            {
                _PathHandler = new PathHandler(MapGenerationScript, AgentsManagerScript, newPath, BaseScript);
                // Событие повторного построения пути
                _PathHandler.NavigationPathHasBeenCorruptedEvent += _CreateNewPath;
                // Событие полной блокировки пути
                _PathHandler.NavigationPathHasBeenBlockedEvent += () =>
                {
                    _PathHandler.Dispose();

                    _PathHandler = null;
                };
            }
            else
            {
                _PathHandler = null;
            }
        }

        public void Dispose()
        {
            _PathHandler?.Dispose();
        }

        public class PathBlockingSequenceElement
        {
            public PathHandler Agent { get; }

            public MapPoint RequestedPoint { get; }

            public PathBlockingSequenceElement RequestingPointOwner { get; set; } = null;
            public PathBlockingSequenceElement RequestedPointOwner { get; set; } = null;

            public PathBlockingSequenceElement(PathHandler agent, MapPoint requestedPoint)
            {
                Agent = agent;
                RequestedPoint = requestedPoint;
            }
        }

        public class PathBlockingSolverHandler
        {
            private ConcurrentDictionary<PathHandler, PathBlockingSequenceElement> _Agents { get; } = new();

            public void AppendAgent(PathHandler agent, MapPoint requestedPoint)
            {
                lock (this)
                {
                    if (!_Agents.ContainsKey(agent))
                    {
                        var newElement = new PathBlockingSequenceElement(agent, requestedPoint);

                        _Agents.AddOrUpdate(agent, (_) => newElement, (_, _) => newElement);
                    }
                }
            }

            public void RemoveAgent(PathHandler agent)
            {
                lock (this)
                {
                    if (_Agents.ContainsKey(agent))
                    {
                        PathBlockingSequenceElement element;

                        _Agents.Remove(agent, out element);

                        if (element.RequestingPointOwner is not null)
                        {
                            element.RequestingPointOwner.RequestedPointOwner = null;
                        }
                    }
                }
            }

            public void SetRequestingSequenceConnection(PathHandler agent, PathHandler requestedPointAgent)
            {
                lock (this)
                {
                    PathBlockingSequenceElement elementFrom = null;
                    PathBlockingSequenceElement elementTo = null;

                    if (_Agents.ContainsKey(agent))
                    {
                        elementFrom = _Agents.GetValueOrDefault(agent);
                    }

                    if (_Agents.ContainsKey(requestedPointAgent))
                    {
                        elementTo = _Agents.GetValueOrDefault(requestedPointAgent);
                    }

                    if (elementFrom is not null
                        && elementTo is not null)
                    {
                        elementTo.RequestingPointOwner = elementFrom;
                        elementFrom.RequestedPointOwner = elementTo;
                    }
                }
            }

            public bool TryToSolvePathBlocking(PathHandler agent)
            {
                lock (this)
                {
                    PathBlockingSequenceElement elementFrom = null;
                    PathBlockingSequenceElement currentElement = null;
                    HashSet<PathBlockingSequenceElement> passedElements = new();

                    if (_Agents.ContainsKey(agent))
                    {
                        elementFrom = _Agents.GetValueOrDefault(agent);
                        currentElement = elementFrom.RequestedPointOwner;
                    }

                    int iterationIndex = 0;

                    while (iterationIndex < _Agents.Count && currentElement is not null)
                    {
                        if (passedElements.Contains(currentElement))
                        {
                            break;
                        }
                        else
                        {
                            passedElements.Add(currentElement);
                        }

                        if (currentElement == elementFrom)
                        {
                            return true;
                        }
                        else
                        {
                            if (currentElement.Agent.IsPathBlocked)
                            {
                                currentElement = currentElement.RequestedPointOwner;
                            }
                            else
                            {
                                break;
                            }
                        }

                        iterationIndex++;
                    }

                    return false;
                }
            }
        }

        public class PathHandler
        {
            private static CollectionMapper<object, PathHandler> _Mapper { get; } = new();
            private static ConcurrentDictionary<MapGenerationScript, PathBlockingSolverHandler> _PathBlockSolvers { get; } = new();

            public enum PathHandlerState
            {
                PathSwitching = 0,
                RoomMoving = 1,
                RoomTransfer = 2,
                PathIsBlocked = 3,
                CollisionIsDetected = 4,
                PathDestinationIsAchived = 5,
                PathIsFinished = 6
            }

            public AgentBaseScript BaseScript { get; }
            public MapGenerationScript MapGenerationScript { get; }
            public AgentManagerScript AgentManagerScript { get; }
            public MapNavigationGraphPath Path { get; }

            public PathHandlerState State { get; private set; } = PathHandlerState.PathSwitching;
            public bool IsMovementBlocked { get; private set; }
            public bool IsTargetRoomAchived { get; private set; } = false;
            public MapPoint NextPoint { get; private set; }
            public MapRoom CurrentRoom { get; private set; }
            public MapObjectOrientationEnum NextPointCorrection { get; private set; } = MapObjectOrientationEnum.Center;
            public bool IsPathBlocked { get; private set; } = false;

            public event Action NavigationPathHasBeenCorruptedEvent;
            public event Action NavigationPathHasBeenBlockedEvent;

            private MapPoint _TheoreticalTargetPoint { get; set; }
            private ArrayIterator<(MapPoint Point, MapRoom Room)> _PathPoints { get; set; }
            private ArrayIterator<MapPoint> _CurrentRoomPath { get; set; } = null;
            private LinkedList<MapPoint> _PathPointsTrace { get; } = new();
            private MapPoint _PreviousPoint { get; set; }
            private BaseTimer _PathReleasingTimer { get; } = new();
            private BaseTimer _CollisionDisapearTimer { get; } = new();

            public PathHandler(MapGenerationScript mapGenerationScript, AgentManagerScript agentManagerScript, MapNavigationGraphPath path, AgentBaseScript baseScript)
            {
                BaseScript = baseScript;
                MapGenerationScript = mapGenerationScript;
                AgentManagerScript = agentManagerScript;
                Path = path;

                _PathPoints = new(Path.PointsSequence);

                _Mapper.Add(BaseScript, this);
                if (!_PathBlockSolvers.ContainsKey(mapGenerationScript))
                {
                    var newSolver = new PathBlockingSolverHandler();

                    _PathBlockSolvers.AddOrUpdate(mapGenerationScript, (_) => newSolver, (_, _) => newSolver);
                }
            }

            public bool Invoke(MapPoint currentPoint, bool isCurrentTargetPointAchived, bool isFinalTargetPointAchived, bool isCollisionDetected)
            {
                if (_PathPointsTrace.Count == 0
                    || !_PathPointsTrace.Last.Value.AreEqual(currentPoint))
                {
                    if (_PathPointsTrace.Count > 0)
                    {
                        MapGenerationScript.MapNavigation.DynamicObstacles.HandleSingle((obstacles) =>
                        {
                            obstacles.Release(_PathPointsTrace.Last.Value, BaseScript);
                        });

                        _PreviousPoint = _PathPointsTrace.Last.Value;
                    }
                    else
                    {
                        _PreviousPoint = currentPoint;
                        _TheoreticalTargetPoint = currentPoint;
                    }

                    _PathPointsTrace.AddLast(currentPoint);

                    NextPointCorrection = MapObjectOrientationEnum.Center;
                }

                if (isCollisionDetected 
                    && State != PathHandlerState.CollisionIsDetected
                    && State != PathHandlerState.PathDestinationIsAchived
                    && State != PathHandlerState.PathIsFinished)
                {
                    _CollisionDisapearTimer.Update(0.5f);
                    State = PathHandlerState.CollisionIsDetected;

                    _HandleCollisionCorrection(currentPoint);

                    MapGenerationScript.MapNavigation.DynamicObstacles.HandleSingle((obstacles) =>
                    {
                        _ReleaseNextPathPoint(obstacles, currentPoint);
                    });
                }

                if (!_PathPoints.Current.Room.DoesRoomContainsPoint(currentPoint)
                    && !_PathPoints.Next.Room.DoesRoomContainsPoint(currentPoint))
                {
                    NavigationPathHasBeenCorruptedEvent?.Invoke();

                    return false;
                }

                CurrentRoom = MapGenerationScript.MapNavigation.GetRoom(currentPoint);
                IsTargetRoomAchived = CurrentRoom.AreEqual(_PathPoints.Last.Room);

                bool previousePathBlockStatus = IsPathBlocked;

                bool isNextPointAchived = NextPoint is null ? isCurrentTargetPointAchived :
                    NextPoint.AreEqual(_PathPoints.Last.Point) ? isFinalTargetPointAchived : isCurrentTargetPointAchived;

                if (_PathPoints.Current.Room.AreEqual(_PathPoints.Next.Room)
                    && !_PathPoints.Current.Room.AreEqual(CurrentRoom))
                {
                    NavigationPathHasBeenCorruptedEvent?.Invoke();

                    return false;
                }

                switch (State)
                {
                    case PathHandlerState.PathSwitching:

                        MapGenerationScript.MapNavigation.DynamicObstacles.HandleSingle((obstacles) =>
                        {
                            _ReleaseNextPathPoint(obstacles, currentPoint);
                        });

                        if (_CurrentRoomPath is not null)
                        {
                            MapGenerationScript.MapNavigation.DynamicObstacles.HandleSingle((obstacles) =>
                            {
                                foreach (var point in _CurrentRoomPath.TakeLeft())
                                {
                                    if (!point.AreEqual(currentPoint))
                                    {
                                        obstacles.Release(point, BaseScript);
                                    }
                                }
                            });

                            _CurrentRoomPath = null;
                        }

                        // Переход на следующую целевую точку
                        if (currentPoint.AreEqual(_PathPoints.Next.Point))
                        {
                            _PathPoints.MoveNext();
                        }

                        if (currentPoint.AreEqual(_PathPoints.Last.Point))
                        {
                            State = PathHandlerState.PathDestinationIsAchived;
                        }
                        else if (_TheoreticalTargetPoint is not null
                                 && _TheoreticalTargetPoint.AreEqual(_PathPoints.Last.Point))
                        {
                            MapGenerationScript.MapNavigation.DynamicObstacles.HandleSingle((obstacles) =>
                            {
                                _ReserveNextPathPoint(obstacles, _TheoreticalTargetPoint);
                            });

                            State = PathHandlerState.PathDestinationIsAchived;
                        }
                        // Находимся в комнате
                        else if (CurrentRoom.AreEqual(_PathPoints.Current.Room)
                                 && CurrentRoom.AreEqual(_PathPoints.Next.Room))
                        {
                            bool corruptionFlag = false;

                            MapGenerationScript.MapNavigation.DynamicObstacles.HandleSingle((obstacles) =>
                            {
                                var currentRoomPath = MapGenerationScript.MapNavigation.NavigationGraph.FindPathInRoomDynamic(currentPoint, _PathPoints.Next.Point, BaseScript, CurrentRoom.AreEqual(_PathPoints.Last.Room));

                                if (currentRoomPath.DynamicPath.Length <= 1)
                                {
                                    if (currentRoomPath.ActualPath.Length >= 2)
                                    {
                                        _TheoreticalTargetPoint = currentRoomPath.ActualPath[1];

                                        State = _HandleBlockReasons(obstacles, currentRoomPath.ActualPath[1], PathHandlerState.RoomMoving);

                                        if (State == PathHandlerState.RoomMoving)
                                        {
                                            _CurrentRoomPath = new(new MapPoint[]
                                            {
                                                currentRoomPath.ActualPath[0]
                                                ,
                                                currentRoomPath.ActualPath[1]
                                            });

                                            _ReserveNextPathPoint(obstacles, _CurrentRoomPath.Next);
                                        }
                                        else
                                        {
                                            _CurrentRoomPath = null;
                                        }
                                    }
                                    else
                                    {
                                        NavigationPathHasBeenBlockedEvent?.Invoke();

                                        corruptionFlag = true;

                                        return;
                                    }
                                }
                                else
                                {
                                    _CurrentRoomPath = new(currentRoomPath.DynamicPath);

                                    _ReserveNextPathPoint(obstacles, _CurrentRoomPath.Next);

                                    State = PathHandlerState.RoomMoving;
                                }
                            });

                            if (corruptionFlag)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            _HandleTransferReasons();
                        }
                        break;

                    case PathHandlerState.RoomMoving:
                        if (NextPoint.AreEqual(_PathPoints.Last.Point))
                        {
                            State = PathHandlerState.PathSwitching;
                        }
                        else if (isNextPointAchived)
                        {
                            MapGenerationScript.MapNavigation.DynamicObstacles.HandleSingle((obstacles) =>
                            {
                                if (!_CurrentRoomPath.MoveNext())
                                {
                                    State = PathHandlerState.PathSwitching;
                                }
                                else
                                {
                                    _ReserveNextPathPoint(obstacles, _CurrentRoomPath.Next);
                                }
                            });
                        }
                        else if (_CurrentRoomPath.FirstOrDefault(point => point.AreEqual(currentPoint)) == null
                                 || (!currentPoint.AreEqual(_CurrentRoomPath.Current) && !currentPoint.AreEqual(_CurrentRoomPath.Next)))
                        {
                            State = PathHandlerState.PathSwitching;
                        }
                        break;

                    case PathHandlerState.RoomTransfer:
                        if (NextPoint.AreEqual(_PathPoints.Last.Point))
                        {
                            State = PathHandlerState.PathSwitching;
                        }
                        else if (isNextPointAchived)
                        {
                            _PathPoints.MoveNext();
                            State = PathHandlerState.PathSwitching;
                        }
                        else if (CurrentRoom.AreEqual(_PathPoints.Next.Room)
                                 && !currentPoint.AreEqual(_PathPoints.Next.Point))
                        {
                            _PathPoints.MoveNext();
                            State = PathHandlerState.PathSwitching;
                        }
                        break;

                    case PathHandlerState.PathIsBlocked:
                        if (_PathReleasingTimer.IsTimeEllapsed)
                        {
                            State = PathHandlerState.PathSwitching;
                        }
                        break;

                    case PathHandlerState.CollisionIsDetected:
                        if (_CollisionDisapearTimer.IsTimeEllapsed
                            || isNextPointAchived)
                        {
                            State = PathHandlerState.PathSwitching;
                        }
                        break;

                    case PathHandlerState.PathDestinationIsAchived:
                        if (isNextPointAchived)
                        {
                            State = PathHandlerState.PathIsFinished;
                        }
                        break;

                    case PathHandlerState.PathIsFinished:
                        break;
                }

                switch (State)
                {
                    case PathHandlerState.PathSwitching:
                        break;

                    case PathHandlerState.RoomMoving:
                        NextPoint = _CurrentRoomPath.Next;
                        IsPathBlocked = false;
                        break;

                    case PathHandlerState.RoomTransfer:
                        NextPoint = _PathPoints.Next.Point;
                        IsPathBlocked = false;
                        break;

                    case PathHandlerState.PathIsBlocked:
                        IsPathBlocked = true;
                        break;

                    case PathHandlerState.CollisionIsDetected:
                        break;

                    case PathHandlerState.PathIsFinished:
                    case PathHandlerState.PathDestinationIsAchived:
                        NextPoint = _PathPoints.Last.Point;
                        IsPathBlocked = false;
                        break;
                }

                if (NextPoint is null)
                {
                    NextPoint = currentPoint;
                }

                IsMovementBlocked = State == PathHandlerState.PathSwitching
                    || State == PathHandlerState.PathIsBlocked
                    || State == PathHandlerState.PathIsFinished;

                _PathReleasingTimer.Invoke();
                _CollisionDisapearTimer.Invoke();

                if (previousePathBlockStatus
                    && !IsPathBlocked)
                {
                    _PathBlockSolvers[MapGenerationScript].RemoveAgent(this);
                }

                return State == PathHandlerState.PathDestinationIsAchived || State == PathHandlerState.PathIsFinished;
            }

            private void _HandleCollisionCorrection(MapPoint currentPoint)
            {
                MapPoint movementDirection;

                if (!NextPoint.AreEqual(currentPoint))
                {
                    movementDirection = NextPoint - currentPoint;
                }
                else if (!_TheoreticalTargetPoint.AreEqual(currentPoint))
                {
                    movementDirection = _TheoreticalTargetPoint - currentPoint;
                }
                else if (!_PreviousPoint.AreEqual(currentPoint))
                {
                    movementDirection = currentPoint - _PreviousPoint;
                }
                else
                {
                    movementDirection = _PathPoints.Next.Point - currentPoint;
                }

                if (NextPointCorrection == MapObjectOrientationEnum.Center)
                {
                    if (movementDirection.X > 0)
                    {
                        if (movementDirection.Y > 0)
                        {
                            NextPointCorrection = MapObjectOrientationEnum.BottomRight;
                        }
                        if (movementDirection.Y < 0)
                        {
                            NextPointCorrection = MapObjectOrientationEnum.BottomLeft;
                        }
                        if (movementDirection.Y == 0)
                        {
                            NextPointCorrection = MapObjectOrientationEnum.Bottom;
                        }
                    }
                    else if (movementDirection.X < 0)
                    {
                        if (movementDirection.Y > 0)
                        {
                            NextPointCorrection = MapObjectOrientationEnum.TopLeft;
                        }
                        if (movementDirection.Y < 0)
                        {
                            NextPointCorrection = MapObjectOrientationEnum.TopRight;
                        }
                        if (movementDirection.Y == 0)
                        {
                            NextPointCorrection = MapObjectOrientationEnum.Top;
                        }
                    }
                    else if (movementDirection.X == 0)
                    {
                        if (movementDirection.Y > 0)
                        {
                            NextPointCorrection = MapObjectOrientationEnum.Left;
                        }
                        if (movementDirection.Y < 0)
                        {
                            NextPointCorrection = MapObjectOrientationEnum.Right;
                        }
                        if (movementDirection.Y == 0)
                        {
                            NextPointCorrection = MapObjectOrientationEnum.Center;
                        }
                    }
                }
                else
                {
                    NextPointCorrection = (MapObjectOrientationEnum)(((int)NextPointCorrection + MapGenerationScript.Rnd.Next(3)) % 9);
                }
            }

            private void _HandleTransferReasons()
            {
                MapGenerationScript.MapNavigation.DynamicObstacles.HandleSingle((obstacles) =>
                {
                    if (obstacles[_PathPoints.Next.Point, BaseScript])
                    {
                        _ReserveNextPathPoint(obstacles, _PathPoints.Next.Point);

                        if (_PathPoints.Next.Point.AreEqual(_PathPoints.Last.Point))
                        {
                            State = PathHandlerState.PathDestinationIsAchived;
                        }
                        else
                        {
                            State = PathHandlerState.RoomTransfer;
                        }
                    }
                    else
                    {
                        _TheoreticalTargetPoint = _PathPoints.Next.Point;

                        State = _HandleBlockReasons(obstacles, _PathPoints.Next.Point, PathHandlerState.RoomTransfer);

                        if (State == PathHandlerState.RoomTransfer)
                        {
                            _ReserveNextPathPoint(obstacles, _PathPoints.Next.Point);

                            if (_PathPoints.Next.Point.AreEqual(_PathPoints.Last.Point))
                            {
                                State = PathHandlerState.PathDestinationIsAchived;
                            }
                        }
                    }
                });
            }

            private PathHandlerState _HandleBlockReasons(MapDynamicObstaclesHandler obstacles, MapPoint targetPoint, PathHandlerState returnState)
            {
                _TheoreticalTargetPoint = targetPoint;
                _PathBlockSolvers[MapGenerationScript].AppendAgent(this, _TheoreticalTargetPoint);

                var anotherOwner = _Mapper.Get(obstacles.GetCellObstacleOwner(targetPoint));

                if (anotherOwner is not null)
                {
                    _PathBlockSolvers[MapGenerationScript].SetRequestingSequenceConnection(this, anotherOwner);

                    if (_PathBlockSolvers[MapGenerationScript].TryToSolvePathBlocking(this))
                    {
                        return returnState;
                    }
                    else
                    {
                        _PathReleasingTimer.Update(0.3f);
                        return PathHandlerState.PathIsBlocked;
                    }
                }
                else
                {
                    _PathReleasingTimer.Update(0.3f);
                    return PathHandlerState.PathIsBlocked;
                }
            }

            private void _ReserveNextPathPoint(MapDynamicObstaclesHandler obstacles, MapPoint point)
            {
                _TheoreticalTargetPoint = point;

                obstacles.ForceTake(_TheoreticalTargetPoint, BaseScript);
            }

            private void _ReleaseNextPathPoint(MapDynamicObstaclesHandler obstacles, MapPoint currentPoint)
            {
                if (_TheoreticalTargetPoint is not null && 
                    !_TheoreticalTargetPoint.AreEqual(currentPoint))
                {
                    obstacles.Release(_TheoreticalTargetPoint, BaseScript);
                }
            }

            public void Dispose()
            {
                MapGenerationScript.MapNavigation.DynamicObstacles.HandleSingle((obstacles) =>
                {
                    foreach (var point in _PathPointsTrace.Take(_PathPointsTrace.Count - 1))
                    {
                        obstacles.Release(point, BaseScript);
                    }

                    _ReleaseNextPathPoint(obstacles, new MapPoint(-1, -1));
                });

                _Mapper.Remove(BaseScript);
                _PathBlockSolvers[MapGenerationScript].RemoveAgent(this);
            }
        }
    }
}
