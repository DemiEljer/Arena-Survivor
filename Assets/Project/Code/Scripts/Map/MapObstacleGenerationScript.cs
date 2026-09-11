using System;
using System.Collections.Generic;
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
            float actualWallLength = mapGenerationScript.CellSize - mapGenerationScript.WallWidth;
            float actualWallHeight = mapGenerationScript.WallHeight;

            void _SetParent(GameObject newObject)
            {
                if (newObject is not null)
                {
                    var thisGameObjectTransformComponent = this.GetComponent<Transform>();
                    var newGameObjectTransformComponent = newObject.GetComponent<Transform>();

                    if (thisGameObjectTransformComponent is not null
                        && newGameObjectTransformComponent is not null)
                    {
                        newGameObjectTransformComponent.parent = thisGameObjectTransformComponent;
                    }
                }
            }

            void _CreateWall(float xLocation, float zLocation, float yRotation)
            {
                var wallLocation = new Vector3(xLocation, mapGenerationScript.WallHeight / 2.0f, zLocation);
                var wallQuanterion = Quaternion.Euler(0, yRotation, 0);
                var wallScale = new Vector3(mapGenerationScript.WallWidth, actualWallHeight, actualWallLength);

                var newWall = _ObjectsFabric.CreateWall(wallLocation, wallQuanterion, wallScale);

                if (newWall is not null)
                {
                    newWall.name = $"WallPillar";
                }

                _SetParent(newWall);
            }

            void _CreateWallPillar(float xLocation, float zLocation)
            {
                var pillarLocation = new Vector3(xLocation, mapGenerationScript.WallHeight / 2.0f, zLocation);
                var pillarQuanterion = Quaternion.Euler(0, 0, 0);
                var pillarScale = new Vector3(mapGenerationScript.WallWidth, actualWallHeight, mapGenerationScript.WallWidth);

                var newPillar = _ObjectsFabric.CreatePillar(pillarLocation, pillarQuanterion, pillarScale);

                if (newPillar is not null)
                {
                    newPillar.name = $"WallPillar";
                }

                _SetParent(newPillar);
            }

            void _CreateFloor(float xLocation, float zLocation)
            {
                var floorLocation = new Vector3(xLocation, 0, zLocation);
                var floorQuanterion = Quaternion.Euler(0, 0, 0);
                var floorScale = new Vector3(mapGenerationScript.CellSize, mapGenerationScript.FloorHeight, mapGenerationScript.CellSize);

                var newFloor = _ObjectsFabric.CreateFloor(floorLocation, floorQuanterion, floorScale);

                if (newFloor is not null)
                {
                    newFloor.name = $"Floor";
                }

                _SetParent(newFloor);
            }

            void _CreateRoof(float xLocation, float zLocation)
            {
                var roofLocation = new Vector3(xLocation, actualWallHeight, zLocation);
                var roofQuanterion = Quaternion.Euler(0, 0, 0);
                var roofScale = new Vector3(mapGenerationScript.CellSize, mapGenerationScript.RoofHeight, mapGenerationScript.CellSize);

                var newRoof = _ObjectsFabric.CreateRoof(roofLocation, roofQuanterion, roofScale);

                if (newRoof is not null)
                {
                    newRoof.name = $"Floor";
                }

                _SetParent(newRoof);
            }

            map.Foreach((point, cell) =>
            {
                if (DoesGenerateWalls)
                {
                    if (cell.BottomWall && point.Y == (map.Height - 1))
                    {
                        _CreateWall((float)point.X * mapGenerationScript.CellSize + mapGenerationScript.CellSize / 2.0f, (float)(point.Y + 1) * mapGenerationScript.CellSize, 90);
                    }
                    if (cell.TopWall)
                    {
                        _CreateWall((float)point.X * mapGenerationScript.CellSize + mapGenerationScript.CellSize / 2.0f, (float)(point.Y) * mapGenerationScript.CellSize, 270);
                    }
                    if (cell.LeftWall)
                    {
                        _CreateWall((float)(point.X) * mapGenerationScript.CellSize, (float)point.Y * mapGenerationScript.CellSize + mapGenerationScript.CellSize / 2.0f, 0);
                    }
                    if (cell.RightWall && point.X == (map.Width - 1))
                    {
                        _CreateWall((float)(point.X + 1) * mapGenerationScript.CellSize, (float)point.Y * mapGenerationScript.CellSize + mapGenerationScript.CellSize / 2.0f, 180);
                    }

                    var prevDiagCell = map.GetCell(point.X - 1, point.Y - 1);

                    if (cell.TopWall || cell.LeftWall || prevDiagCell.BottomWall || prevDiagCell.RightWall)
                    {
                        _CreateWallPillar((float)point.X * mapGenerationScript.CellSize, (float)point.Y * mapGenerationScript.CellSize);
                    }

                    if ((cell.TopWall || cell.RightWall) && point.X == (map.Width - 1))
                    {
                        _CreateWallPillar((float)(point.X + 1) * mapGenerationScript.CellSize, (float)point.Y * mapGenerationScript.CellSize);
                    }

                    if ((cell.BottomWall || cell.RightWall) && point.Y == (map.Height - 1))
                    {
                        _CreateWallPillar((float)(point.X + 1) * mapGenerationScript.CellSize, (float)(point.Y + 1) * mapGenerationScript.CellSize);
                    }
                }

                if (DoesGenerateFloor)
                {
                    _CreateFloor((float)point.X * mapGenerationScript.CellSize + mapGenerationScript.CellSize / 2.0f, (float)point.Y * mapGenerationScript.CellSize + mapGenerationScript.CellSize / 2.0f);
                }

                if (DoesGenerateRoof)
                {
                    _CreateRoof((float)point.X * mapGenerationScript.CellSize + mapGenerationScript.CellSize / 2.0f, (float)point.Y * mapGenerationScript.CellSize + mapGenerationScript.CellSize / 2.0f);
                }
            });
        }
    }
}
