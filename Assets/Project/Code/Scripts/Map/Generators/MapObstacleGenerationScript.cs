using Assets.Project.Code.Scripts.Map.Fabrics;
using MapGenearionLibrary;
using MapGenearionLibrary.Base;
using MapGenearionLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Code.Scripts.Map.Generators
{
    public class MapObstacleGenerationScript : AMapObjectGenerationScript
    {
        public bool DoesGenerateWalls = false;
        public bool DoesGenerateFloor = false;
        public bool DoesGenerateRoof = false;
        public bool DoesGenerateDoors = false;

        private MapSimpleObstacleObjectFabricScript _SimpleObjectsFabric { get; set; }
        private MapComplexObstaclesFabricScript _ComplexObjectsFabric { get; set; }

        protected override void _Start()
        {
            _SimpleObjectsFabric = GetComponent<MapSimpleObstacleObjectFabricScript>();
            _ComplexObjectsFabric = GetComponent<MapComplexObstaclesFabricScript>();

            _RegistrateFabric(_SimpleObjectsFabric);
            _RegistrateFabric(_ComplexObjectsFabric);
        }

        protected override void _Generate(MapGenerationScript mapGenerationScript)
        {
            if (mapGenerationScript.Params.SimpleGenerationMode)
            {
                _SimpleModeGeneration(mapGenerationScript);
            }
            else
            {
                _ComplexModeGeneration(mapGenerationScript);
            }
        }

        private void _SimpleModeGeneration(MapGenerationScript mapGenerationScript)
        {
            if (_SimpleObjectsFabric is null)
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

                var newWall = _SimpleObjectsFabric.CreateWall(wallLocation, wallQuanterion, wallScale);

                if (newWall is not null)
                {
                    newWall.name = $"Wall";
                }
            }

            void _CreateWallPillar(MapPoint point, MapObjectOrientationEnum pillarOrientation)
            {
                Vector3 pillarLocation = mapGenerationScript.ObjectLocations.GetPillarLocation(point, pillarOrientation);
                var pillarQuanterion = Quaternion.Euler(0, 0, 0);
                var pillarScale = new Vector3(mapGenerationScript.Params.WallWidth, actualWallHeight, mapGenerationScript.Params.WallWidth);

                var newPillar = _SimpleObjectsFabric.CreatePillar(pillarLocation, pillarQuanterion, pillarScale);

                if (newPillar is not null)
                {
                    newPillar.name = $"WallPillar";
                }
            }

            void _CreateFloor(MapPoint point)
            {
                Vector3 floorLocation = mapGenerationScript.ObjectLocations.GetFloorLocation(point); ;
                var floorQuanterion = Quaternion.Euler(0, 0, 0);
                var floorScale = new Vector3(mapGenerationScript.Params.CellSize, mapGenerationScript.Params.FloorHeight, mapGenerationScript.Params.CellSize);

                var newFloor = _SimpleObjectsFabric.CreateFloor(floorLocation, floorQuanterion, floorScale);

                if (newFloor is not null)
                {
                    newFloor.name = $"Floor";
                }
            }

            void _CreateRoof(MapPoint point)
            {
                Vector3 roofLocation = mapGenerationScript.ObjectLocations.GetRoofLocation(point);
                var roofQuanterion = Quaternion.Euler(0, 0, 0);
                var roofScale = new Vector3(mapGenerationScript.Params.CellSize, mapGenerationScript.Params.RoofHeight, mapGenerationScript.Params.CellSize);

                var newRoof = _SimpleObjectsFabric.CreateRoof(roofLocation, roofQuanterion, roofScale);

                if (newRoof is not null)
                {
                    newRoof.name = $"Roof";
                }
            }

            void _CreateDoor(MapPoint point, MapObjectOrientationEnum doorOrientation)
            {
                Vector3 doorLocation = mapGenerationScript.ObjectLocations.GetWallLocation(point, doorOrientation);
                Quaternion doorQuanterion = mapGenerationScript.ObjectLocations.GetObjectRotation(doorOrientation);
                var doorScale = new Vector3(mapGenerationScript.Params.WallWidth, actualWallHeight, actualWallLength);

                var newWall = _SimpleObjectsFabric.CreateDoor(doorLocation, doorQuanterion, doorScale);

                if (newWall is not null)
                {
                    newWall.name = $"Door";
                }
            }

            map.Foreach((point, cell) =>
            {
                if (DoesGenerateWalls)
                {
                    if (point.Y == (map.Height - 1))
                    {
                        if (cell.BottomWall)
                        {
                            _CreateWall(point, MapObjectOrientationEnum.Bottom);
                        }
                    }

                    if (cell.TopWall)
                    {
                        _CreateWall(point, MapObjectOrientationEnum.Top);
                    }
                    if (cell.LeftWall)
                    {
                        _CreateWall(point, MapObjectOrientationEnum.Left);
                    }
                    if (point.X == (map.Width - 1))
                    {
                        if (cell.RightWall)
                        {
                            _CreateWall(point, MapObjectOrientationEnum.Right);
                        }
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

            if (DoesGenerateDoors)
            {
                foreach (var door in mapGenerationScript.MapNavigation.Doors)
                {
                    _CreateDoor(door.Area1, (MapObjectOrientationEnum)door.Orientation);

                    if (door.Orientation == CellWallOrientationEnum.Left)
                    {
                        var prevCell = map.GetCell(door.Area1.X, door.Area1.Y - 1);
                        var nextCell = map.GetCell(door.Area1.X, door.Area1.Y + 1);

                        if (!prevCell.GetWall(CellWallOrientationEnum.Left))
                        {
                            _CreateWallPillar(door.Area1, MapObjectOrientationEnum.TopLeft);
                        }
                        if (!prevCell.GetWall(CellWallOrientationEnum.Left))
                        {
                            _CreateWallPillar(door.Area1, MapObjectOrientationEnum.BottomLeft);
                        }
                    }
                    else if (door.Orientation == CellWallOrientationEnum.Top)
                    {
                        var prevCell = map.GetCell(door.Area1.X - 1, door.Area1.Y);
                        var nextCell = map.GetCell(door.Area1.X + 1, door.Area1.Y);

                        if (!prevCell.GetWall(CellWallOrientationEnum.Top))
                        {
                            _CreateWallPillar(door.Area1, MapObjectOrientationEnum.TopRight);
                        }
                        if (!prevCell.GetWall(CellWallOrientationEnum.Top))
                        {
                            _CreateWallPillar(door.Area1, MapObjectOrientationEnum.TopLeft);
                        }
                    }
                }
            }
        }

        private void _ComplexModeGeneration(MapGenerationScript mapGenerationScript)
        {
            if (_ComplexObjectsFabric is null)
            {
                return;
            }

            var map = mapGenerationScript.Map;
            float actualWallLength = mapGenerationScript.Params.CellSize - mapGenerationScript.Params.WallWidth;
            float actualWallHeight = mapGenerationScript.Params.WallHeight;

            void _CreateWall(MapPoint point, MapObjectOrientationEnum wallOrientation, MapObjectOrientationEnum wallSide, bool isCorner)
            {
                Vector3 wallLocation = mapGenerationScript.ObjectLocations.GetWallLocation(point, wallOrientation, wallSide);

                Quaternion wallQuanterion = mapGenerationScript.ObjectLocations.GetObjectRotation(wallOrientation);
                var wallScale = new Vector3(mapGenerationScript.Params.WallWidth, actualWallHeight, actualWallLength);

                GameObject newWall;

                if (isCorner)
                {
                    if (wallSide == MapObjectOrientationEnum.Left)
                    {
                        newWall = _ComplexObjectsFabric.CreateWallCornerLeft(wallLocation, wallQuanterion, wallScale);
                    }
                    else
                    {
                        newWall = _ComplexObjectsFabric.CreateWallCornerRight(wallLocation, wallQuanterion, wallScale);
                    }
                }
                else
                {
                    if (wallSide == MapObjectOrientationEnum.Left)
                    {
                        newWall = _ComplexObjectsFabric.CreateWallFlatLeft(wallLocation, wallQuanterion, wallScale);
                    }
                    else
                    {
                        newWall = _ComplexObjectsFabric.CreateWallFlatRight(wallLocation, wallQuanterion, wallScale);
                    }
                }

                if (newWall is not null)
                {
                    newWall.name = $"Wall";
                }
            }

            void _CreateRoof(MapPoint point, MapObjectOrientationEnum roofLocationOrientation, MapCell cell)
            {
                Vector3 roofLocation = mapGenerationScript.ObjectLocations.GetRoofLocation(point, roofLocationOrientation);
                var roofScale = new Vector3(mapGenerationScript.Params.CellSize, mapGenerationScript.Params.RoofHeight, mapGenerationScript.Params.CellSize);

                GameObject newRoof = null;

                switch (roofLocationOrientation)
                {
                    case MapObjectOrientationEnum.TopRight:
                        if (cell.TopWall && cell.RightWall)
                        {
                            var roofQuanterion = mapGenerationScript.ObjectLocations.GetObjectRotation(MapObjectOrientationEnum.Right);
                            newRoof = _ComplexObjectsFabric.CreateRoofCornerRight(roofLocation, roofQuanterion, roofScale);
                        }
                        else if (cell.TopWall)
                        {
                            var roofQuanterion = mapGenerationScript.ObjectLocations.GetObjectRotation(MapObjectOrientationEnum.Top);
                            newRoof = _ComplexObjectsFabric.CreateRoofFlat(roofLocation, roofQuanterion, roofScale);
                        }
                        else if (cell.RightWall)
                        {
                            var roofQuanterion = mapGenerationScript.ObjectLocations.GetObjectRotation(MapObjectOrientationEnum.Right);
                            newRoof = _ComplexObjectsFabric.CreateRoofFlat(roofLocation, roofQuanterion, roofScale);
                        }
                        else
                        {
                            var roofQuanterion = mapGenerationScript.ObjectLocations.GetObjectRotation(MapObjectOrientationEnum.Right);
                            newRoof = _ComplexObjectsFabric.CreateRoofNoWalls(roofLocation, roofQuanterion, roofScale);
                        }
                        break;

                    case MapObjectOrientationEnum.TopLeft:
                        if (cell.TopWall && cell.LeftWall)
                        {
                            var roofQuanterion = mapGenerationScript.ObjectLocations.GetObjectRotation(MapObjectOrientationEnum.Left);
                            newRoof = _ComplexObjectsFabric.CreateRoofCornerLeft(roofLocation, roofQuanterion, roofScale);
                        }
                        else if (cell.TopWall)
                        {
                            var roofQuanterion = mapGenerationScript.ObjectLocations.GetObjectRotation(MapObjectOrientationEnum.Top);
                            newRoof = _ComplexObjectsFabric.CreateRoofFlat(roofLocation, roofQuanterion, roofScale);
                        }
                        else if (cell.LeftWall)
                        {
                            var roofQuanterion = mapGenerationScript.ObjectLocations.GetObjectRotation(MapObjectOrientationEnum.Left);
                            newRoof = _ComplexObjectsFabric.CreateRoofFlat(roofLocation, roofQuanterion, roofScale);
                        }
                        else
                        {
                            var roofQuanterion = mapGenerationScript.ObjectLocations.GetObjectRotation(MapObjectOrientationEnum.Left);
                            newRoof = _ComplexObjectsFabric.CreateRoofNoWalls(roofLocation, roofQuanterion, roofScale);
                        }
                        break;

                    case MapObjectOrientationEnum.BottomLeft:
                        if (cell.BottomWall && cell.LeftWall)
                        {
                            var roofQuanterion = mapGenerationScript.ObjectLocations.GetObjectRotation(MapObjectOrientationEnum.Bottom);
                            newRoof = _ComplexObjectsFabric.CreateRoofCornerLeft(roofLocation, roofQuanterion, roofScale);

                        }
                        else if (cell.BottomWall)
                        {
                            var roofQuanterion = mapGenerationScript.ObjectLocations.GetObjectRotation(MapObjectOrientationEnum.Bottom);
                            newRoof = _ComplexObjectsFabric.CreateRoofFlat(roofLocation, roofQuanterion, roofScale);
                        }
                        else if (cell.LeftWall)
                        {
                            var roofQuanterion = mapGenerationScript.ObjectLocations.GetObjectRotation(MapObjectOrientationEnum.Left);
                            newRoof = _ComplexObjectsFabric.CreateRoofFlat(roofLocation, roofQuanterion, roofScale);
                        }
                        else
                        {
                            var roofQuanterion = mapGenerationScript.ObjectLocations.GetObjectRotation(MapObjectOrientationEnum.Left);
                            newRoof = _ComplexObjectsFabric.CreateRoofNoWalls(roofLocation, roofQuanterion, roofScale);
                        }
                        break;

                    case MapObjectOrientationEnum.BottomRight:
                        if (cell.BottomWall && cell.RightWall)
                        {
                            var roofQuanterion = mapGenerationScript.ObjectLocations.GetObjectRotation(MapObjectOrientationEnum.Bottom);
                            newRoof = _ComplexObjectsFabric.CreateRoofCornerRight(roofLocation, roofQuanterion, roofScale);
                        }
                        else if (cell.BottomWall)
                        {
                            var roofQuanterion = mapGenerationScript.ObjectLocations.GetObjectRotation(MapObjectOrientationEnum.Bottom);
                            newRoof = _ComplexObjectsFabric.CreateRoofFlat(roofLocation, roofQuanterion, roofScale);
                        }
                        else if (cell.RightWall)
                        {
                            var roofQuanterion = mapGenerationScript.ObjectLocations.GetObjectRotation(MapObjectOrientationEnum.Right);
                            newRoof = _ComplexObjectsFabric.CreateRoofFlat(roofLocation, roofQuanterion, roofScale);
                        }
                        else
                        {
                            var roofQuanterion = mapGenerationScript.ObjectLocations.GetObjectRotation(MapObjectOrientationEnum.Right);
                            newRoof = _ComplexObjectsFabric.CreateRoofNoWalls(roofLocation, roofQuanterion, roofScale);
                        }
                        break;
                }


                if (newRoof is not null)
                {
                    newRoof.name = $"Roof";
                }
            }

            void _CreateFloor(MapPoint point)
            {
                Vector3 floorLocation = mapGenerationScript.ObjectLocations.GetFloorLocation(point); ;
                var floorQuanterion = Quaternion.Euler(0, 0, 0);
                var floorScale = new Vector3(mapGenerationScript.Params.CellSize, mapGenerationScript.Params.FloorHeight, mapGenerationScript.Params.CellSize);

                var newFloor = _ComplexObjectsFabric.CreateFloor(floorLocation, floorQuanterion, floorScale);

                if (newFloor is not null)
                {
                    newFloor.name = $"Floor";
                }
            }

            void _CreateWallPillar(MapPoint point, MapObjectOrientationEnum pillarOrientationLocation, MapObjectOrientationEnum pillarOrientation, bool isInternal)
            {
                Vector3 pillarLocation = mapGenerationScript.ObjectLocations.GetPillarLocation(point, pillarOrientationLocation);
                var pillarQuanterion = mapGenerationScript.ObjectLocations.GetObjectRotation(pillarOrientation);
                var pillarScale = new Vector3(mapGenerationScript.Params.WallWidth, actualWallHeight, mapGenerationScript.Params.WallWidth);

                GameObject newPillar;

                if (isInternal)
                {
                    newPillar = _ComplexObjectsFabric.CreatePillarIn(pillarLocation, pillarQuanterion, pillarScale);
                }
                else
                {
                    newPillar = _ComplexObjectsFabric.CreatePillarOut(pillarLocation, pillarQuanterion, pillarScale);
                }

                if (newPillar is not null)
                {
                    newPillar.name = $"WallPillar";
                }
            }

            void _CreateDoor(MapPoint point, MapObjectOrientationEnum doorOrientation)
            {
                Vector3 doorLocation = mapGenerationScript.ObjectLocations.GetWallLocation(point, doorOrientation);
                Quaternion doorQuanterion = mapGenerationScript.ObjectLocations.GetObjectRotation(doorOrientation);
                var doorScale = new Vector3(mapGenerationScript.Params.WallWidth, actualWallHeight, actualWallLength);

                var newWall = _ComplexObjectsFabric.CreateAcrh(doorLocation, doorQuanterion, doorScale);

                if (newWall is not null)
                {
                    newWall.name = $"Arch";
                }
            }

            map.Foreach((point, cell) =>
            {
                if (DoesGenerateWalls)
                {
                    if (cell.LeftWall)
                    {
                        _CreateWall(point, MapObjectOrientationEnum.Left, MapObjectOrientationEnum.Left, cell.BottomWall);
                        _CreateWall(point, MapObjectOrientationEnum.Left, MapObjectOrientationEnum.Right, cell.TopWall);
                    }
                    if (cell.TopWall)
                    {
                        _CreateWall(point, MapObjectOrientationEnum.Top, MapObjectOrientationEnum.Left, cell.LeftWall);
                        _CreateWall(point, MapObjectOrientationEnum.Top, MapObjectOrientationEnum.Right, cell.RightWall);
                    }
                    if (cell.RightWall)
                    {
                        _CreateWall(point, MapObjectOrientationEnum.Right, MapObjectOrientationEnum.Left, cell.TopWall);
                        _CreateWall(point, MapObjectOrientationEnum.Right, MapObjectOrientationEnum.Right, cell.BottomWall);
                    }
                    if (cell.BottomWall)
                    {
                        _CreateWall(point, MapObjectOrientationEnum.Bottom, MapObjectOrientationEnum.Left, cell.RightWall);
                        _CreateWall(point, MapObjectOrientationEnum.Bottom, MapObjectOrientationEnum.Right, cell.LeftWall);
                    }

                    var prevDiagCell = map.GetCell(point.X - 1, point.Y - 1);
                    var nextDiagCell = map.GetCell(point.X + 1, point.Y + 1);

                    if (cell.TopWall || cell.LeftWall || prevDiagCell.BottomWall || prevDiagCell.RightWall)
                    {
                        _CreateWallPillar(point, MapObjectOrientationEnum.TopLeft, MapObjectOrientationEnum.Left, cell.TopWall);
                        _CreateWallPillar(point, MapObjectOrientationEnum.TopLeft, MapObjectOrientationEnum.Top, cell.LeftWall);
                    }

                    if (cell.BottomWall || cell.RightWall || nextDiagCell.TopWall || nextDiagCell.LeftWall)
                    {
                        _CreateWallPillar(point, MapObjectOrientationEnum.BottomRight, MapObjectOrientationEnum.Right, cell.BottomWall);
                        _CreateWallPillar(point, MapObjectOrientationEnum.BottomRight, MapObjectOrientationEnum.Bottom, cell.RightWall);
                    }

                    if ((cell.TopWall || cell.RightWall) && (point.X == (map.Width - 1)))
                    {
                        _CreateWallPillar(point, MapObjectOrientationEnum.TopRight, MapObjectOrientationEnum.Right, cell.TopWall);
                        _CreateWallPillar(point, MapObjectOrientationEnum.TopRight, MapObjectOrientationEnum.Top, cell.RightWall);
                    }

                    if ((cell.BottomWall || cell.LeftWall) && (point.Y == (map.Height - 1)))
                    {
                        _CreateWallPillar(point, MapObjectOrientationEnum.BottomLeft, MapObjectOrientationEnum.Left, cell.BottomWall);
                        _CreateWallPillar(point, MapObjectOrientationEnum.BottomLeft, MapObjectOrientationEnum.Bottom, cell.LeftWall);
                    }
                }

                if (DoesGenerateFloor)
                {
                    _CreateFloor(point);
                }

                if (DoesGenerateRoof)
                {
                    _CreateRoof(point, MapObjectOrientationEnum.TopLeft, cell);
                    _CreateRoof(point, MapObjectOrientationEnum.TopRight, cell);
                    _CreateRoof(point, MapObjectOrientationEnum.BottomLeft, cell);
                    _CreateRoof(point, MapObjectOrientationEnum.BottomRight, cell);
                }
            });

            if (DoesGenerateDoors)
            {
                foreach (var door in mapGenerationScript.MapNavigation.Doors)
                {
                    _CreateDoor(door.Area1, (MapObjectOrientationEnum)door.Orientation);

                    if (door.Orientation == CellWallOrientationEnum.Left)
                    {
                        var nextCellTop = map.GetCell(door.Area1.X, door.Area1.Y - 1);
                        var nextCellBottom = map.GetCell(door.Area1.X, door.Area1.Y + 1);

                        if (!nextCellTop.LeftWall && !nextCellTop.BottomWall)
                        {
                            _CreateWallPillar(door.Area1, MapObjectOrientationEnum.TopLeft, MapObjectOrientationEnum.Top, false);
                        }
                        if (!nextCellBottom.LeftWall && !nextCellBottom.TopWall)
                        {
                            _CreateWallPillar(door.Area1, MapObjectOrientationEnum.BottomLeft, MapObjectOrientationEnum.Bottom, false);
                        }

                        var nextCellLeft = map.GetCell(door.Area1.X - 1, door.Area1.Y - 1);
                        var nextCellRight = map.GetCell(door.Area1.X - 1, door.Area1.Y);

                        if (!nextCellTop.BottomWall && !nextCellTop.LeftWall 
                            && mapGenerationScript.MapNavigation.Doors.FirstOrDefault(d => d.DoesDoorContainsPoint(nextCellTop.Point) && d.DoesDoorContainsPoint(door.Area1)) == null)
                        {
                            _CreateWallPillar(door.Area1, MapObjectOrientationEnum.TopLeft, MapObjectOrientationEnum.Left, false);
                        }

                        if (!nextCellLeft.BottomWall && !nextCellLeft.RightWall
                            && mapGenerationScript.MapNavigation.Doors.FirstOrDefault(d => d.DoesDoorContainsPoint(nextCellLeft.Point) && d.DoesDoorContainsPoint(nextCellRight.Point)) == null)
                        {
                            _CreateWallPillar(door.Area1, MapObjectOrientationEnum.TopLeft, MapObjectOrientationEnum.Right, false);
                        }
                    }
                    else
                    {
                        var nextCellLeft = map.GetCell(door.Area1.X - 1, door.Area1.Y);
                        var nextCellRight = map.GetCell(door.Area1.X + 1, door.Area1.Y);

                        if (!nextCellLeft.TopWall && !nextCellLeft.RightWall)
                        {
                            _CreateWallPillar(door.Area1, MapObjectOrientationEnum.TopLeft, MapObjectOrientationEnum.Left, false);
                        }

                        if (!nextCellRight.TopWall && !nextCellRight.LeftWall)
                        {
                            _CreateWallPillar(door.Area1, MapObjectOrientationEnum.TopRight, MapObjectOrientationEnum.Right, false);
                        }

                        var nextCellTop = map.GetCell(door.Area1.X - 1, door.Area1.Y - 1);
                        var nextCellBottom = map.GetCell(door.Area1.X, door.Area1.Y - 1);

                        if (!nextCellLeft.RightWall && !nextCellLeft.TopWall
                            && mapGenerationScript.MapNavigation.Doors.FirstOrDefault(d => d.DoesDoorContainsPoint(nextCellLeft.Point) && d.DoesDoorContainsPoint(door.Area1)) == null)
                        {
                            _CreateWallPillar(door.Area1, MapObjectOrientationEnum.TopLeft, MapObjectOrientationEnum.Bottom, false);
                        }

                        if (!nextCellTop.RightWall && !nextCellTop.BottomWall
                            && mapGenerationScript.MapNavigation.Doors.FirstOrDefault(d => d.DoesDoorContainsPoint(nextCellTop.Point) && d.DoesDoorContainsPoint(nextCellBottom.Point)) == null)
                        {
                            _CreateWallPillar(door.Area1, MapObjectOrientationEnum.TopLeft, MapObjectOrientationEnum.Top, false);
                        }
                    }
                }
            }
        }
    }
}
