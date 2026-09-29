using Assets.Project.Code.Scripts.Map.Fabrics;
using Assets.Project.Code.Scripts.Map.Help;
using MapGenearionLibrary.Base;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Code.Scripts.Map.Generators
{
    public class MapStaticObjectGenerationScript : AMapObjectGenerationScript
    {
        public bool DoesGenerateObjects = false;
        public float DensityFactor = 0.9f;
        public float WallOffset = 0.5f;

        private MapStaticObjectFabricScript _Fabric { get; set; }

        protected override void _Start()
        {
            _Fabric = GetComponent<MapStaticObjectFabricScript>();

            _RegistrateFabric(_Fabric);
        }

        protected override void _Generate(MapGenerationScript mapGenerationScript)
        {
            if (!DoesGenerateObjects || _Fabric is null) return;

            MapCellDensityCalculation cellsDensity = new(mapGenerationScript.Map, DensityFactor);
            // Исключение точек
            {
                foreach (var door in mapGenerationScript.MapNavigation.Doors)
                {
                    cellsDensity.Maximize(door.Area1);
                    cellsDensity.Maximize(door.Area2);
                }

                foreach (var room in mapGenerationScript.MapNavigation.Rooms)
                {
                    if (room.Height == 1)
                    {
                        foreach (var index in Enumerable.Range(0, room.Width))
                        {
                            cellsDensity.Maximize(room.StartX + index, room.StartY);
                        }
                    }
                    else if (room.Width == 1)
                    {
                        foreach (var index in Enumerable.Range(0, room.Height))
                        {
                            cellsDensity.Maximize(room.StartX, room.StartY + index);
                        }
                    }
                }
            }

            void _CreateObject(MapPoint point)
            {
                var room = mapGenerationScript.MapNavigation.Rooms.FirstOrDefault(r => r.DoesRoomContainsPoint(point));

                if (room is not null)
                {
                    if (point.X == room.StartX)
                    {
                        _CreateWallObject(point, MapObjectOrientationEnum.Left);
                    }
                    else if (point.Y == room.StartY)
                    {
                        _CreateWallObject(point, MapObjectOrientationEnum.Top);
                    }
                    else if (point.X == (room.StartX + room.Width - 1))
                    {
                        _CreateWallObject(point, MapObjectOrientationEnum.Right);
                    }
                    else if (point.X == (room.StartY + room.Height - 1))
                    {
                        _CreateWallObject(point, MapObjectOrientationEnum.Bottom);
                    }
                    else
                    {
                        _CreateCellCentralObject(point);
                    }
                }
            }

            void _CreateCellCentralObject(MapPoint point)
            {
                Vector3 objectLocation = mapGenerationScript.ObjectLocations.GetCellCentralFloorLocation(point);
                Quaternion objectQuanterion = mapGenerationScript.ObjectLocations.GetObjectRotation((MapObjectOrientationEnum)(MapGenerationScript.Rnd.Next(4)));

                var newWall = _Fabric.CreateCentralCellObject(objectLocation, objectQuanterion);

                mapGenerationScript.MapNavigation.Obstacles[point] = true;

                if (newWall is not null)
                {
                    newWall.name = $"StaticObject";
                }
            }

            void _CreateWallObject(MapPoint point, MapObjectOrientationEnum wallOrientation)
            {
                Vector3 objectLocation = mapGenerationScript.ObjectLocations.GetWallFloorSurfaceLocation(point, wallOrientation, WallOffset);
                Quaternion objectQuanterion = mapGenerationScript.ObjectLocations.GetObjectRotation(wallOrientation);

                var newWall = _Fabric.CreateWallObject(objectLocation, objectQuanterion);

                mapGenerationScript.MapNavigation.Obstacles[point] = true;

                if (newWall is not null)
                {
                    newWall.name = $"StaticObject";
                }
            }

            while (!cellsDensity.IsFullfilled)
            {
                var targetPoint = cellsDensity.GetMinValueCells().First();

                _CreateObject(targetPoint);

                cellsDensity.Set(targetPoint);
            }
        }
    }
}
