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
        public const float PointSameLocationDistance = 0.3f;
        public MapGenerationScript MapGenerationScript { get; }

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
                var isTargetMapPointAchived = _PathHandler.Invoke(IsCurrentTargetLocationAchived());

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
                CurrentTargetLocation = CurrentLocation;
            }
        }

        public bool IsTargetLocationAchived() => _CompareToLocations(CurrentLocation, TargetLocation);

        public bool IsCurrentTargetLocationAchived() => _CompareToLocations(CurrentLocation, CurrentTargetLocation);

        private bool _CompareToLocations(Vector3 location1, Vector3 location2)
        {
            location1.y = 0;
            location2.y = 0;

            return Vector3.Distance(location1, location2) < PointSameLocationDistance;
        }

        private void _CreateNewPath()
        {
            var newPath = MapGenerationScript.MapNavigation.NavigationGraph.GetPathes(CurrentMapPoint, TargetMapPoint).FirstOrDefault();

            if (newPath is not null)
            {
                _PathHandler = new PathHandler(newPath);
            }
            else
            {
                _PathHandler = null;
            }
        }

        public class PathHandler
        {
            public MapNavigationGraphPath _Path { get; }

            public MapPoint NextPoint => _Path.PointsSequence[_PathElementIndex];

            private int _PathElementIndex = 0;

            public PathHandler(MapNavigationGraphPath path)
            {
                _Path = path;
            }

            public bool Invoke(bool isCurrentTargetPointAchived)
            {
                if (isCurrentTargetPointAchived)
                {
                    if (_PathElementIndex < (_Path.PointsSequence.Length - 1))
                    {
                        _PathElementIndex++;

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
        }
    }
}
