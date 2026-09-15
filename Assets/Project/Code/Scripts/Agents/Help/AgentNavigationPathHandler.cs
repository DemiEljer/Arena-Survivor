using Assets.Project.Code.Scripts.Map;
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
        public float PathCollisionDetectionDistance { get; set; } = 0.2f;
        public MapGenerationScript MapGenerationScript { get; }

        public float DistanceToObstacle { get; set; }
        public Vector3 CurrentLocation { get; private set; }
        public MapPoint CurrentMapPoint { get; private set; }
        public Vector3 TargetLocation { get; private set; }
        public MapPoint TargetMapPoint { get; private set; }
        public Vector3 CurrentTargetLocation { get; private set; }
        public MapPoint CurrentTargetMapPoint { get; private set; }

        private PathHandler _PathHandler { get; set; } = null;

        public AgentNavigationPathHandler(MapGenerationScript mapGenerationScript)
        {
            MapGenerationScript = mapGenerationScript;
        }

        public void SetCurrentLocation(Vector3 currentLocation)
        {
            CurrentLocation = currentLocation;
            CurrentMapPoint = MapGenerationScript.ObjectLocations.GetCellMapPoint(CurrentLocation);
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
            if (_PathHandler is not null)
            {
                var isTargetMapPointAchived = _PathHandler.Invoke(CurrentMapPoint, IsCurrentTargetLocationAchived(), DistanceToObstacle);
                
                _PathHandler.PathCollisionDetectionDistance = PathCollisionDetectionDistance;

                CurrentTargetMapPoint = _PathHandler.NextPoint;

                if (isTargetMapPointAchived)
                {
                    CurrentTargetLocation = TargetLocation;
                }
                else
                {
                    CurrentTargetLocation = MapGenerationScript.ObjectLocations.GetCellCentralLocation(CurrentTargetMapPoint);
                }
                // Небольшая модификация точки назначения в рамках выбранной клетки
                if (_PathHandler.IsCollisionDeteced)
                {
                    CurrentTargetLocation = MapGenerationScript.ObjectLocations.AmplitudeCellLocationModification(CurrentTargetLocation);
                }
            }
            else
            {
                CurrentTargetMapPoint = CurrentMapPoint;
                CurrentTargetLocation = CurrentLocation;
            }
        }

        public bool IsTargetLocationAchived() => MapObjectLocationHandler.CompareTwoLocations(CurrentLocation, TargetLocation, PathCollisionDetectionDistance);

        public bool IsCurrentTargetLocationAchived() => MapObjectLocationHandler.CompareTwoLocations(CurrentLocation, CurrentTargetLocation, PathCollisionDetectionDistance);

        private void _CreateNewPath()
        {
            var newPath = MapGenerationScript.MapNavigation.NavigationGraph.GetPathes(CurrentMapPoint, TargetMapPoint).FirstOrDefault();

            if (newPath is not null)
            {
                _PathHandler = new PathHandler(newPath);
                // Событие повторного построения пути
                _PathHandler.NavigationPathHasBeenCorruptedEvent += _CreateNewPath;
            }
            else
            {
                _PathHandler = null;
            }
        }

        public class PathHandler
        {
            public MapNavigationGraphPath _Path { get; }

            public MapPoint NextPoint
            {
                get
                {
                    if (_CurrentHelpPoint is null)
                    {
                        return _Path.PointsSequence[_PathPointIndex];
                    }
                    else
                    {
                        return _CurrentHelpPoint;
                    }
                }
            }
            public MapRoom CurrentRoom => _Path.RoomsSequence[_PathRoomIndex];
            public float PathCollisionDetectionDistance { get; set; }
            public bool IsCollisionDeteced { get; private set; } = false;

            public event Action NavigationPathHasBeenCorruptedEvent;

            private int _PathPointIndex = 0;
            private int _PathRoomIndex = 0;

            private MapPoint _CurrentHelpPoint { get; set; } = null;

            public PathHandler(MapNavigationGraphPath path)
            {
                _Path = path;
            }

            public bool Invoke(MapPoint currentPoint, bool isCurrentTargetPointAchived, float distanceToObstacle)
            {
                IsCollisionDeteced = false;

                if (!CurrentRoom.DoesRoomContainsPoint(currentPoint))
                {
                    if (_PathRoomIndex < (_Path.RoomsSequence.Length - 1))
                    {
                        _PathRoomIndex++;
                        // Произошло событие сбития с пути
                        if (!CurrentRoom.DoesRoomContainsPoint(currentPoint))
                        {
                            NavigationPathHasBeenCorruptedEvent?.Invoke();
                        }
                    }
                }
                // В случае, если происходит коллизия
                if (distanceToObstacle < PathCollisionDetectionDistance)
                {
                    _FindNearbyRoomHelpPoint(currentPoint);

                    IsCollisionDeteced = true;

                    return false;
                }
                else if (isCurrentTargetPointAchived)
                {
                    if (_CurrentHelpPoint is not null)
                    {
                        _CurrentHelpPoint = null;

                        return false;
                    }
                    else if (_PathPointIndex < (_Path.PointsSequence.Length - 1))
                    {
                        _PathPointIndex++;

                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }

            private void _FindNearbyRoomHelpPoint(MapPoint currentPoint)
            {
                int deltaX = 0;

                if (CurrentRoom.Width != 1)
                {
                    if (currentPoint.X == (CurrentRoom.StartX + CurrentRoom.Width - 1))
                    {
                        deltaX -= MapGenerationScript.Rnd.Next(0, 2);
                    }
                    else if (currentPoint.X == CurrentRoom.StartX)
                    {
                        deltaX += MapGenerationScript.Rnd.Next(0, 2);
                    }
                    else
                    {
                        deltaX += MapGenerationScript.Rnd.Next(0, 3) - 1;
                    }
                }

                int deltaY = 0;

                if (CurrentRoom.Height != 1)
                {
                    if (currentPoint.Y == (CurrentRoom.StartY + CurrentRoom.Height - 1))
                    {
                        deltaY -= MapGenerationScript.Rnd.Next(0, 2);
                    }
                    else if (currentPoint.Y == CurrentRoom.StartY)
                    {
                        deltaY += MapGenerationScript.Rnd.Next(0, 2);
                    }
                    else
                    {
                        deltaY += MapGenerationScript.Rnd.Next(0, 3) - 1;
                    }
                }

                _CurrentHelpPoint = new MapPoint(currentPoint.X + deltaX, currentPoint.Y + deltaY);
            }
        }
    }
}
