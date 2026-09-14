using Assets.Project.Code.Scripts.Map.Fabrics;
using Assets.Project.Code.Scripts.Map.Help;
using MapGenearionLibrary.Base;
using MapGenearionLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Code.Scripts.Map.Generators
{
    public class MapLightObjectGenerationScript : AMapObjectGenerationScript
    {
        public bool DoesGenerateLightPoints = false;
        public bool DoesGenerateTorches = false;

        public float LightPointsDensity = 0.5f;
        public int TorchPeriod = 5;

        private MapLightObjectFabricScript _ObjectsFabric { get; set; }

        private System.Random _Rnd { get; } = new System.Random();

        private MapCellDensityCalculation _PointLightDensity { get; set; }

        protected override void _Start()
        {
            _ObjectsFabric = GetComponent<MapLightObjectFabricScript>();

            _PointLightDensity = new MapCellDensityCalculation(_MapGenerationScript.Map, LightPointsDensity);
        }

        protected override void _Generate(MapGenerationScript mapGenerationScript)
        {
            if (_ObjectsFabric is null)
            {
                return;
            }

            void _CreateLightPoint(MapPoint lightLocation)
            {
                var newLightObject = _ObjectsFabric.CreateLightPoint(mapGenerationScript.ObjectLocations.GetCellCentralLocation(lightLocation));
                newLightObject.name = "PointLight";

                mapGenerationScript.AppendObjectAsChild(newLightObject);
            }

            void _CreateTorchesInRoom(MapRoom room)
            {
                int cellIndex = 0;
                int torchPeriod = Math.Max(1, TorchPeriod);

                void _HandleTorchCreatingLogic(MapPoint point, MapObjectOrientationEnum orientation)
                {
                    var isWall = mapGenerationScript.Map.GetCell(point.X, point.Y).GetWall((CellWallOrientationEnum)(orientation));

                    if ((cellIndex % torchPeriod) == 0 && isWall)
                    {
                        _CreateTorch(point, orientation);

                        cellIndex++;
                    }
                    else if (isWall)
                    {
                        cellIndex++;
                    }
                }

                for (int i = 0; i < room.Width; i++)
                {
                    _HandleTorchCreatingLogic(new MapPoint(room.StartX + i, room.StartY), MapObjectOrientationEnum.Top);
                }

                for (int i = 0; i < room.Height; i++)
                {
                    _HandleTorchCreatingLogic(new MapPoint(room.StartX + room.Width - 1, room.StartY + i), MapObjectOrientationEnum.Right);
                }

                if (room.Height > 1)
                {
                    for (int i = 0; i < room.Width; i++)
                    {
                        _HandleTorchCreatingLogic(new MapPoint(room.StartX + room.Width - 1 - i, room.StartY + room.Height - 1), MapObjectOrientationEnum.Bottom);
                    }
                }

                if (room.Width > 1)
                {
                    for (int i = 0; i < room.Height - 1; i++)
                    {
                        _HandleTorchCreatingLogic(new MapPoint(room.StartX, room.StartY + room.Height - 1 - i), MapObjectOrientationEnum.Left);
                    }
                }
            }

            void _CreateTorch(MapPoint torchLocation, MapObjectOrientationEnum orientation)
            {
                var newTorchObject = _ObjectsFabric.CreateTorch
                (
                    mapGenerationScript.ObjectLocations.GetWallCentralSurfaceLocation(torchLocation, orientation)
                    ,
                    mapGenerationScript.ObjectLocations.GetObjectRotation(orientation)
                );
                newTorchObject.name = "Torch";

                mapGenerationScript.AppendObjectAsChild(newTorchObject);
            }

            if (DoesGenerateLightPoints)
            {
                while (!_PointLightDensity.IsFullfilled)
                {
                    var vaicantPoint = _PointLightDensity.GetMinValueCells().First();

                    if (vaicantPoint is not null)
                    {
                        _PointLightDensity.Set(vaicantPoint);

                        _CreateLightPoint(vaicantPoint);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (DoesGenerateTorches)
            {
                foreach (var room in mapGenerationScript.MapNavigation.Rooms)
                {
                    _CreateTorchesInRoom(room);
                }
            }
        }
    }
}
