using MapGenearionLibrary.Base;
using MapGenearionLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Code.Scripts.Map
{
    public class MapObstacleGenerationScript : AMapObjectGenerationScript
    {
        public bool DoesGenerateWalls = false;

        public bool DoesGenerateFloor = false;

        public bool DoesGenerateRoof = false;

        private MapObstacleObjectFabricScript _ObjectsFabric { get; set; }

        protected override void _Start()
        {
            _ObjectsFabric = GetComponent<MapObstacleObjectFabricScript>();
        }

        protected override void _Generate(MapGenerationScript mapGenerationScript)
        {
            if (_ObjectsFabric is null)
            {
                return;
            }

            var map = mapGenerationScript.Map;
            float actualWallLength = mapGenerationScript.Params.CellSize - mapGenerationScript.Params.WallWidth;
            float actualWallHeight = mapGenerationScript.Params.WallHeight;

            void _CreateWall(MapPoint point, MapObjectOrientationEnum wallOrientation)
            {
                Vector3 wallLocation = mapGenerationScript.ObjectLocations.GetWallLocation(point, wallOrientation);
                Quaternion wallQuanterion = mapGenerationScript.ObjectLocations.GetObjectRotation(wallOrientation);
                var wallScale = new Vector3(mapGenerationScript.Params.WallWidth, actualWallHeight, actualWallLength);

                var newWall = _ObjectsFabric.CreateWall(wallLocation, wallQuanterion, wallScale);

                if (newWall is not null)
                {
                    newWall.name = $"Wall";
                }

                mapGenerationScript.AppendObjectAsChild(newWall);
            }

            void _CreateWallPillar(MapPoint point, MapObjectOrientationEnum pillarOrientation)
            {
                Vector3 pillarLocation = mapGenerationScript.ObjectLocations.GetPillarLocation(point, pillarOrientation);
                var pillarQuanterion = Quaternion.Euler(0, 0, 0);
                var pillarScale = new Vector3(mapGenerationScript.Params.WallWidth, actualWallHeight, mapGenerationScript.Params.WallWidth);

                var newPillar = _ObjectsFabric.CreatePillar(pillarLocation, pillarQuanterion, pillarScale);

                if (newPillar is not null)
                {
                    newPillar.name = $"WallPillar";
                }

                mapGenerationScript.AppendObjectAsChild(newPillar);
            }

            void _CreateFloor(MapPoint point)
            {
                Vector3 floorLocation = mapGenerationScript.ObjectLocations.GetFloorLocation(point); ;
                var floorQuanterion = Quaternion.Euler(0, 0, 0);
                var floorScale = new Vector3(mapGenerationScript.Params.CellSize, mapGenerationScript.Params.FloorHeight, mapGenerationScript.Params.CellSize);

                var newFloor = _ObjectsFabric.CreateFloor(floorLocation, floorQuanterion, floorScale);

                if (newFloor is not null)
                {
                    newFloor.name = $"Floor";
                }

                mapGenerationScript.AppendObjectAsChild(newFloor);
            }

            void _CreateRoof(MapPoint point)
            {
                Vector3 roofLocation = mapGenerationScript.ObjectLocations.GetRoofLocation(point);
                var roofQuanterion = Quaternion.Euler(0, 0, 0);
                var roofScale = new Vector3(mapGenerationScript.Params.CellSize, mapGenerationScript.Params.RoofHeight, mapGenerationScript.Params.CellSize);

                var newRoof = _ObjectsFabric.CreateRoof(roofLocation, roofQuanterion, roofScale);

                if (newRoof is not null)
                {
                    newRoof.name = $"Roof";
                }

                mapGenerationScript.AppendObjectAsChild(newRoof);
            }

            map.Foreach((point, cell) =>
            {
                if (DoesGenerateWalls)
                {
                    if (cell.BottomWall && point.Y == (map.Height - 1))
                    {
                        _CreateWall(point, MapObjectOrientationEnum.Bottom);
                    }
                    if (cell.TopWall)
                    {
                        _CreateWall(point, MapObjectOrientationEnum.Top);
                    }
                    if (cell.LeftWall)
                    {
                        _CreateWall(point, MapObjectOrientationEnum.Left);
                    }
                    if (cell.RightWall && point.X == (map.Width - 1))
                    {
                        _CreateWall(point, MapObjectOrientationEnum.Right);
                    }

                    var prevDiagCell = map.GetCell(point.X - 1, point.Y - 1);

                    if (cell.TopWall || cell.LeftWall || prevDiagCell.BottomWall || prevDiagCell.RightWall)
                    {
                        _CreateWallPillar(point, MapObjectOrientationEnum.TopLeft);
                    }

                    if ((cell.TopWall || cell.RightWall) && point.X == (map.Width - 1))
                    {
                        _CreateWallPillar(point, MapObjectOrientationEnum.TopRight);
                    }

                    if ((cell.BottomWall || cell.RightWall) && point.Y == (map.Height - 1))
                    {
                        _CreateWallPillar(point, MapObjectOrientationEnum.BottomRight);
                    }
                }

                if (DoesGenerateFloor)
                {
                    _CreateFloor(point);
                }

                if (DoesGenerateRoof)
                {
                    _CreateRoof(point);
                }
            });
        }
    }
}
