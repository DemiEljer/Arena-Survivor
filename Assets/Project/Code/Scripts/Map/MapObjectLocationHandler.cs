using MapGenearionLibrary.Base;
using MapGenearionLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Code.Scripts.Map
{
    public class MapObjectLocationHandler
    {
        private MapGenerationScript _MapGenerationScript;

        public MapObjectLocationHandler(MapGenerationScript mapGenerationScript)
        {
            _MapGenerationScript = mapGenerationScript;
        }

        public Vector3 GetWallLocation(MapPoint point, MapObjectOrientationEnum wallOrientation)
        {
            switch (wallOrientation)
            {
                case MapObjectOrientationEnum.Left:
                    return new Vector3(
                    (float)(point.X) * _MapGenerationScript.Params.CellSize
                    ,
                    _MapGenerationScript.Params.WallHeight / 2.0f
                    ,
                    (float)(point.Y) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f);

                case MapObjectOrientationEnum.Bottom:
                    return new Vector3(
                    (float)(point.X) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f
                    ,
                    _MapGenerationScript.Params.WallHeight / 2.0f
                    ,
                    (float)(point.Y + 1) * _MapGenerationScript.Params.CellSize);

                case MapObjectOrientationEnum.Right:
                    return new Vector3(
                    (float)(point.X + 1) * _MapGenerationScript.Params.CellSize
                    ,
                    _MapGenerationScript.Params.WallHeight / 2.0f
                    ,
                    (float)(point.Y) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f);

                case MapObjectOrientationEnum.Top:
                    return new Vector3(
                    (float)(point.X) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f
                    ,
                    _MapGenerationScript.Params.WallHeight / 2.0f
                    ,
                    (float)(point.Y) * _MapGenerationScript.Params.CellSize);

                default: throw new ArgumentException();
            }
        }

        public Vector3 GetWallLocation(MapPoint point, MapObjectOrientationEnum wallOrientation, MapObjectOrientationEnum wallSide)
        {
            switch (wallOrientation)
            {
                case MapObjectOrientationEnum.Left:
                    if (wallSide == MapObjectOrientationEnum.Left)
                    {
                        return new Vector3(
                            (float)(point.X) * _MapGenerationScript.Params.CellSize
                            ,
                            _MapGenerationScript.Params.WallHeight / 2.0f
                            ,
                            (float)(point.Y) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f + _MapGenerationScript.Params.CellSize / 4.0f - _MapGenerationScript.Params.WallWidth / 4.0f);
                    }
                    else
                    {
                        return new Vector3(
                            (float)(point.X) * _MapGenerationScript.Params.CellSize
                            ,
                            _MapGenerationScript.Params.WallHeight / 2.0f
                            ,
                            (float)(point.Y) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f - _MapGenerationScript.Params.CellSize / 4.0f + _MapGenerationScript.Params.WallWidth / 4.0f);
                    }


                case MapObjectOrientationEnum.Bottom:
                    if (wallSide == MapObjectOrientationEnum.Left)
                    {
                        return new Vector3(
                            (float)(point.X) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f + _MapGenerationScript.Params.CellSize / 4.0f - _MapGenerationScript.Params.WallWidth / 4.0f
                            ,
                            _MapGenerationScript.Params.WallHeight / 2.0f
                            ,
                            (float)(point.Y + 1) * _MapGenerationScript.Params.CellSize);
                    }
                    else
                    {
                        return new Vector3(
                            (float)(point.X) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f - _MapGenerationScript.Params.CellSize / 4.0f + _MapGenerationScript.Params.WallWidth / 4.0f
                            ,
                            _MapGenerationScript.Params.WallHeight / 2.0f
                            ,
                            (float)(point.Y + 1) * _MapGenerationScript.Params.CellSize);
                    }


                case MapObjectOrientationEnum.Right:
                    if (wallSide == MapObjectOrientationEnum.Left)
                    {
                        return new Vector3(
                            (float)(point.X + 1) * _MapGenerationScript.Params.CellSize
                            ,
                            _MapGenerationScript.Params.WallHeight / 2.0f
                            ,
                            (float)(point.Y) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f - _MapGenerationScript.Params.CellSize / 4.0f + _MapGenerationScript.Params.WallWidth / 4.0f);
                    }
                    else
                    {
                        return new Vector3(
                            (float)(point.X + 1) * _MapGenerationScript.Params.CellSize
                            ,
                            _MapGenerationScript.Params.WallHeight / 2.0f
                            ,
                            (float)(point.Y) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f + _MapGenerationScript.Params.CellSize / 4.0f - _MapGenerationScript.Params.WallWidth / 4.0f);
                    }


                case MapObjectOrientationEnum.Top:
                    if (wallSide == MapObjectOrientationEnum.Left)
                    {
                        return new Vector3(
                            (float)(point.X) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f - _MapGenerationScript.Params.CellSize / 4.0f + _MapGenerationScript.Params.WallWidth / 4.0f
                            ,
                            _MapGenerationScript.Params.WallHeight / 2.0f
                            ,
                            (float)(point.Y) * _MapGenerationScript.Params.CellSize);
                    }
                    else
                    {
                        return new Vector3(
                            (float)(point.X) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f + _MapGenerationScript.Params.CellSize / 4.0f - _MapGenerationScript.Params.WallWidth / 4.0f
                            ,
                            _MapGenerationScript.Params.WallHeight / 2.0f
                            ,
                            (float)(point.Y) * _MapGenerationScript.Params.CellSize);
                    }


                default: throw new ArgumentException();
            }
        }

        public Quaternion GetObjectRotation(MapObjectOrientationEnum wallOrientation)
        {
            switch (wallOrientation)
            {
                case MapObjectOrientationEnum.Left: return Quaternion.Euler(0, 0, 0);
                case MapObjectOrientationEnum.Bottom: return Quaternion.Euler(0, 90, 0);
                case MapObjectOrientationEnum.Right: return Quaternion.Euler(0, 180, 0);
                case MapObjectOrientationEnum.Top: return Quaternion.Euler(0, 270, 0);
                default: throw new ArgumentException();
            }
        }

        public Vector3 GetWallCentralSurfaceLocation(MapPoint point, MapObjectOrientationEnum wallOrientation, Bounds objectBounds, float offset = 0.0f)
        {
            if (wallOrientation == MapObjectOrientationEnum.Left || wallOrientation == MapObjectOrientationEnum.Right)
            {
                return GetWallCentralSurfaceLocation(point, wallOrientation, objectBounds.size.x / 2.0f + offset);
            }
            else if (wallOrientation == MapObjectOrientationEnum.Top || wallOrientation == MapObjectOrientationEnum.Bottom)
            {
                return GetWallCentralSurfaceLocation(point, wallOrientation, objectBounds.size.z / 2.0f + offset);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public Vector3 GetWallCentralSurfaceLocation(MapPoint point, MapObjectOrientationEnum wallOrientation, float offset = 0.0f)
        {
            switch (wallOrientation)
            {
                case MapObjectOrientationEnum.Left:
                    return new Vector3(
                    (float)(point.X) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.WallWidth / 2.0f + (_MapGenerationScript.Params.SimpleGenerationMode ? 0.0f : _MapGenerationScript.Params.WallWidth * 0.3f) + offset
                    ,
                    _MapGenerationScript.Params.WallHeight / 2.0f
                    ,
                    (float)(point.Y) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f);

                case MapObjectOrientationEnum.Bottom:
                    return new Vector3(
                    (float)(point.X) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f
                    ,
                    _MapGenerationScript.Params.WallHeight / 2.0f
                    ,
                    (float)(point.Y + 1) * _MapGenerationScript.Params.CellSize - _MapGenerationScript.Params.WallWidth / 2.0f - (_MapGenerationScript.Params.SimpleGenerationMode ? 0.0f : _MapGenerationScript.Params.WallWidth * 0.3f) - offset);

                case MapObjectOrientationEnum.Right:
                    return new Vector3(
                    (float)(point.X + 1) * _MapGenerationScript.Params.CellSize - _MapGenerationScript.Params.WallWidth / 2.0f - (_MapGenerationScript.Params.SimpleGenerationMode ? 0.0f : _MapGenerationScript.Params.WallWidth * 0.3f) - offset
                    ,
                    _MapGenerationScript.Params.WallHeight / 2.0f
                    ,
                    (float)(point.Y) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f);

                case MapObjectOrientationEnum.Top:
                    return new Vector3(
                    (float)(point.X) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f
                    ,
                    _MapGenerationScript.Params.WallHeight / 2.0f
                    ,
                    (float)(point.Y) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.WallWidth / 2.0f + (_MapGenerationScript.Params.SimpleGenerationMode ? 0.0f : _MapGenerationScript.Params.WallWidth * 0.3f) + offset);

                default: throw new ArgumentException();
            }
        }

        public Vector3 GetWallFloorSurfaceLocation(MapPoint point, MapObjectOrientationEnum wallOrientation, Bounds objectBounds, float offset = 0.0f)
        {
            if (wallOrientation == MapObjectOrientationEnum.Left || wallOrientation == MapObjectOrientationEnum.Right)
            {
                return GetWallFloorSurfaceLocation(point, wallOrientation, objectBounds.size.x / 2.0f + offset);
            }
            else if (wallOrientation == MapObjectOrientationEnum.Top || wallOrientation == MapObjectOrientationEnum.Bottom)
            {
                return GetWallFloorSurfaceLocation(point, wallOrientation, objectBounds.size.z / 2.0f + offset);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public Vector3 GetWallFloorSurfaceLocation(MapPoint point, MapObjectOrientationEnum wallOrientation, float offset = 0.0f)
        {
            switch (wallOrientation)
            {
                case MapObjectOrientationEnum.Left:
                    return new Vector3(
                    (float)(point.X) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.WallWidth / 2.0f + (_MapGenerationScript.Params.SimpleGenerationMode ? 0.0f : _MapGenerationScript.Params.WallWidth * 0.3f) + offset
                    ,
                    0
                    ,
                    (float)(point.Y) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f);

                case MapObjectOrientationEnum.Bottom:
                    return new Vector3(
                    (float)(point.X) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f
                    ,
                    0
                    ,
                    (float)(point.Y + 1) * _MapGenerationScript.Params.CellSize - _MapGenerationScript.Params.WallWidth / 2.0f - (_MapGenerationScript.Params.SimpleGenerationMode ? 0.0f : _MapGenerationScript.Params.WallWidth * 0.3f) - offset);

                case MapObjectOrientationEnum.Right:
                    return new Vector3(
                    (float)(point.X + 1) * _MapGenerationScript.Params.CellSize - _MapGenerationScript.Params.WallWidth / 2.0f - (_MapGenerationScript.Params.SimpleGenerationMode ? 0.0f : _MapGenerationScript.Params.WallWidth * 0.3f) - offset
                    ,
                    0
                    ,
                    (float)(point.Y) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f);

                case MapObjectOrientationEnum.Top:
                    return new Vector3(
                    (float)(point.X) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f
                    ,
                    0
                    ,
                    (float)(point.Y) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.WallWidth / 2.0f + (_MapGenerationScript.Params.SimpleGenerationMode ? 0.0f : _MapGenerationScript.Params.WallWidth * 0.3f) + offset);

                default: throw new ArgumentException();
            }
        }

        public Vector3 GetPillarLocation(MapPoint point, MapObjectOrientationEnum pillarOrientation)
        {
            switch (pillarOrientation)
            {
                case MapObjectOrientationEnum.TopLeft:
                    return new Vector3(
                    (float)(point.X) * _MapGenerationScript.Params.CellSize
                    ,
                    _MapGenerationScript.Params.WallHeight / 2.0f
                    ,
                    (float)(point.Y) * _MapGenerationScript.Params.CellSize);

                case MapObjectOrientationEnum.TopRight:
                    return new Vector3(
                    (float)(point.X + 1) * _MapGenerationScript.Params.CellSize
                    ,
                    _MapGenerationScript.Params.WallHeight / 2.0f
                    ,
                    (float)(point.Y) * _MapGenerationScript.Params.CellSize);

                case MapObjectOrientationEnum.BottomLeft:
                    return new Vector3(
                    (float)(point.X) * _MapGenerationScript.Params.CellSize
                    ,
                    _MapGenerationScript.Params.WallHeight / 2.0f
                    ,
                    (float)(point.Y + 1) * _MapGenerationScript.Params.CellSize);

                case MapObjectOrientationEnum.BottomRight:
                    return new Vector3(
                    (float)(point.X + 1) * _MapGenerationScript.Params.CellSize
                    ,
                    _MapGenerationScript.Params.WallHeight / 2.0f
                    ,
                    (float)(point.Y + 1) * _MapGenerationScript.Params.CellSize);

                default: throw new ArgumentException();
            }
        }

        public Vector3 GetCellCentralLocation(MapPoint point) => new Vector3(
            (float)(point.X) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f
            ,
            _MapGenerationScript.Params.WallHeight / 2.0f
            ,
            (float)(point.Y) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f);

        public Vector3 GetCellCentralFloorLocation(MapPoint point) => new Vector3(
            (float)(point.X) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f
            ,
            0
            ,
            (float)(point.Y) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f);

        public MapPoint GetCellMapPoint(Vector3 location) => new MapPoint(
            (int)(location.x / _MapGenerationScript.Params.CellSize)
            ,
            (int)(location.z / _MapGenerationScript.Params.CellSize)
            );

        public Vector3 GetFloorLocation(MapPoint point) => new Vector3(
            (float)(point.X) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f
            ,
            0
            ,
            (float)point.Y * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f);

        public Vector3 GetRoofLocation(MapPoint point) => new Vector3(
            (float)(point.X) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f
            ,
            _MapGenerationScript.Params.WallHeight
            ,
            (float)(point.Y) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f);

        public Vector3 GetRoofLocation(MapPoint point, MapObjectOrientationEnum orientation)
        {
            switch (orientation)
            {
                case MapObjectOrientationEnum.TopLeft:
                    return new Vector3(
                        (float)(point.X) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f - _MapGenerationScript.Params.CellSize / 4.0f
                        ,
                        _MapGenerationScript.Params.WallHeight
                        ,
                        (float)(point.Y) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f - _MapGenerationScript.Params.CellSize / 4.0f);

                case MapObjectOrientationEnum.TopRight:
                    return new Vector3(
                        (float)(point.X) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f + _MapGenerationScript.Params.CellSize / 4.0f
                        ,
                        _MapGenerationScript.Params.WallHeight
                        ,
                        (float)(point.Y) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f - _MapGenerationScript.Params.CellSize / 4.0f);

                case MapObjectOrientationEnum.BottomLeft:
                    return new Vector3(
                        (float)(point.X) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f - _MapGenerationScript.Params.CellSize / 4.0f
                        ,
                        _MapGenerationScript.Params.WallHeight
                        ,
                        (float)(point.Y) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f + _MapGenerationScript.Params.CellSize / 4.0f);

                case MapObjectOrientationEnum.BottomRight:
                    return new Vector3(
                        (float)(point.X) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f + _MapGenerationScript.Params.CellSize / 4.0f
                        ,
                        _MapGenerationScript.Params.WallHeight
                        ,
                        (float)(point.Y) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f + _MapGenerationScript.Params.CellSize / 4.0f);

                default: throw new ArgumentException();
            }
        }

        public Vector3 GetFloorCentralSurfaceLocation(MapPoint point) => new Vector3(
            (float)(point.X) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f
            ,
            _MapGenerationScript.Params.FloorHeight / 2.0f
            ,
            (float)(point.Y) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f);

        public Vector3 GetRoofCentralSurfaceLocation(MapPoint point) => new Vector3(
            (float)(point.X) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f
            ,
            _MapGenerationScript.Params.WallHeight - _MapGenerationScript.Params.RoofHeight / 2.0f
            ,
            (float)(point.Y) * _MapGenerationScript.Params.CellSize + _MapGenerationScript.Params.CellSize / 2.0f);

        public Vector3 AmplitudeCellLocationModification(Vector3 location) => new Vector3
        (
            location.x + (float)MapGenerationScript.Rnd.NextDouble() * _MapGenerationScript.Params.CellSize - _MapGenerationScript.Params.CellSize / 2.0f
            ,
            location.y
            ,
            location.z + (float)MapGenerationScript.Rnd.NextDouble() * _MapGenerationScript.Params.CellSize - _MapGenerationScript.Params.CellSize / 2.0f
        );

        public static bool CompareTwoLocations(Vector3 location1, Vector3 location2, float approximation)
        {
            location1.y = 0;
            location2.y = 0;

            return Vector3.Distance(location1, location2) < approximation;
        }
    }
}
