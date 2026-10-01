using Assets.Project.Code.Scripts.Map;
using Assets.Project.Code.Scripts.Map.Help;
using MapGenearionLibrary.Base;
using MapGenearionLibrary.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Assets.Project.Code.Scripts.Agents.Help.AgentNavigationPathHandler;

namespace Assets.Project.Code.Scripts.Agents.Help
{
    public class AgentNavigationPathHandler
    {
        public AgentManagerScript AgentsManagerScript { get; }
        public MapGenerationScript MapGenerationScript { get; }

        public float DistanceToObstacle { get; set; }
        public Vector3 CurrentLocation { get; private set; }
        public MapPoint CurrentMapPoint { get; private set; }
        public Vector3 TargetLocation { get; private set; }
        public MapPoint TargetMapPoint { get; private set; }
        public Vector3 CurrentTargetLocation { get; private set; }
        public MapPoint CurrentTargetMapPoint { get; private set; }

        public bool IsCollisionDetected { get; private set; }
        public bool IsNoPath => _PathHandler is null;

        private PathHandler _PathHandler { get; set; } = null;
        private MapObstacleObjectAssociation _MapLocationAssociation { get; set; }

        public AgentNavigationPathHandler(AgentManagerScript agentsManager, MapGenerationScript mapGenerationScript)
        {
            AgentsManagerScript = agentsManager;
            MapGenerationScript = mapGenerationScript;
            _MapLocationAssociation = new MapObstacleObjectAssociation(mapGenerationScript);
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
                    CurrentTargetLocation = MapGenerationScript.ObjectLocations.GetCellCentralLocation(CurrentTargetMapPoint);
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
            var newPath = MapGenerationScript.MapNavigation.NavigationGraph.GetPathes(CurrentMapPoint, TargetMapPoint, AgentsManagerScript.Params.PathSearchingDepth).FirstOrDefault();

            if (newPath is not null)
            {
                _PathHandler = new PathHandler(MapGenerationScript, AgentsManagerScript, newPath);
                // Событие повторного построения пути
                _PathHandler.NavigationPathHasBeenCorruptedEvent += _CreateNewPath;
                // Событие полной блокировки пути
                _PathHandler.NavigationPathHasBeenBlockedEvent += () => _PathHandler = null;
            }
            else
            {
                _PathHandler = null;
            }
        }

        public class PathHandler
        {
            public MapGenerationScript MapGenerationScript { get; }
            public AgentManagerScript AgentManagerScript { get; }
            public MapNavigationGraphPath Path { get; }

            public MapPoint NextPoint
            {
                get
                {
                    if (_RandomRoomPoint is not null)
                    {
                        return _RandomRoomPoint;
                    }
                    else if (_CurrentHelpRoomPoints is null)
                    {
                        return Path.PointsSequence[_PathPointIndex].point;
                    }
                    else
                    {
                        return _CurrentHelpRoomPoints[_HelpRoomPathIndex];
                    }
                }
            }
            public MapRoom CurrentRoom => _PathPointIndex > 0 ? Path.PointsSequence[_PathPointIndex - 1].room : NextRoom;
            public MapRoom NextRoom => Path.PointsSequence[_PathPointIndex].room;

            public event Action NavigationPathHasBeenCorruptedEvent;
            public event Action NavigationPathHasBeenBlockedEvent;

            private int _PathPointIndex = 0;
            private int _HelpRoomPathIndex = 0;

            private MapPoint[] _CurrentHelpRoomPoints { get; set; } = null;
            private MapPoint _RandomRoomPoint { get; set; } = null;
            private bool _PrevCollisionStatus { get; set; } = false;
            private MapPoint _PrevPoint
            {
                get
                {
                    if (_PathTrace.Count == 0)
                    {
                        return new MapPoint();
                    }
                    else if (_PathTrace.Count == 1)
                    {
                        return _PathTrace[0];
                    }
                    else
                    {
                        return _PathTrace[_PathTrace.Count - 2];
                    }
                }
            }
            private MapPoint _LastPoint
            {
                get
                {
                    if (_PathTrace.Count == 0)
                    {
                        return new MapPoint();
                    }
                    else
                    {
                        return _PathTrace.Last();
                    }
                }
            }
            private List<MapPoint> _PathTrace { get; } = new();

            public PathHandler(MapGenerationScript mapGenerationScript, AgentManagerScript agentManagerScript, MapNavigationGraphPath path)
            {
                MapGenerationScript = mapGenerationScript;
                AgentManagerScript = agentManagerScript;
                Path = path;
            }

            public bool Invoke(MapPoint currentPoint, bool isCurrentTargetPointAchived, bool isFinalTargetPointAchived, bool isCollisionDetected)
            {
                if (!CurrentRoom.DoesRoomContainsPoint(currentPoint) 
                    && !NextRoom.DoesRoomContainsPoint(currentPoint))
                {
                    NavigationPathHasBeenCorruptedEvent?.Invoke();

                    return false;
                }

                if (isCollisionDetected && !_PrevCollisionStatus)
                {
                    _RandomRoomPoint = null;
                    _CurrentHelpRoomPoints = null;

                    if (_FindRoomHelpPath()) {}
                    else if (_FindRandomPoint())
                    {
                        if (_PathTrace.Where(tracePoint => tracePoint.AreEqual(_RandomRoomPoint)).Count() > AgentManagerScript.Params.MaxCollisionsCount)
                        {
                            NavigationPathHasBeenBlockedEvent?.Invoke();
                        }
                    }
                    else
                    {
                        NavigationPathHasBeenBlockedEvent?.Invoke();
                    }
                }
                _PrevCollisionStatus = isCollisionDetected;

                // Алгоритм разрешения передвижения
                if ((isCurrentTargetPointAchived && !NextPoint.AreEqual(Path.PointTo))
                    || (isFinalTargetPointAchived && NextPoint.AreEqual(Path.PointTo)))
                {
                    if (_RandomRoomPoint is not null)
                    {
                        _RandomRoomPoint = null;
                    }
                    else if (_CurrentHelpRoomPoints is not null)
                    {
                        if (_HelpRoomPathIndex < (_CurrentHelpRoomPoints.Length - 1))
                        {
                            _HelpRoomPathIndex++;
                        }
                        else
                        {
                            _CurrentHelpRoomPoints = null;
                        }
                    }
                    else
                    {
                        if (_PathPointIndex < (Path.PointsSequence.Length - 1))
                        {
                            _PathPointIndex++;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }

                if (!_LastPoint.AreEqual(currentPoint))
                {
                    _PathTrace.Add(currentPoint);
                }

                return false;
            }

            private bool _FindRoomHelpPath()
            {
                _HelpRoomPathIndex = 0;

                var inRoomNavigationPath = MapGenerationScript.MapNavigation.NavigationGraph.FindPathInRoom(_PrevPoint, NextPoint);

                if (inRoomNavigationPath.Length > 0)
                {
                    _CurrentHelpRoomPoints = inRoomNavigationPath;
                }
                else
                {
                    _CurrentHelpRoomPoints = null;
                }

                return _CurrentHelpRoomPoints is not null;
            }

            private bool _FindRandomPoint()
            {
                _RandomRoomPoint = null;

                var randomPoints = MapGenerationScript.MapNavigation.NavigationGraph.FindDirectionInRoom(_PrevPoint, NextPoint);

                if (randomPoints.Length > 0)
                {
                    _RandomRoomPoint = randomPoints[MapGenerationScript.Rnd.Next(0, randomPoints.Length)];

                    if (!NextRoom.AreEqual(CurrentRoom))
                    {
                        _PathPointIndex = Math.Max(0, _PathPointIndex - 1);
                    }
                }

                return _RandomRoomPoint is not null;
            }
        }
    }
}
