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
                case MapObjectOrientationEnum.Left: return new Vector3(
                    (float)(point.X) * _MapGenerationScript.CellSize
                    , 
                    _MapGenerationScript.WallHeight / 2.0f
                    , 
                    (float)(point.Y) * _MapGenerationScript.CellSize + _MapGenerationScript.CellSize / 2.0f);

                case MapObjectOrientationEnum.Bottom: return new Vector3(
                    (float)(point.X) * _MapGenerationScript.CellSize + _MapGenerationScript.CellSize / 2.0f
                    ,
                    _MapGenerationScript.WallHeight / 2.0f
                    ,
                    (float)(point.Y + 1) * _MapGenerationScript.CellSize);

                case MapObjectOrientationEnum.Right: return new Vector3(
                    (float)(point.X + 1) * _MapGenerationScript.CellSize
                    ,
                    _MapGenerationScript.WallHeight / 2.0f
                    ,
                    (float)(point.Y) * _MapGenerationScript.CellSize + _MapGenerationScript.CellSize / 2.0f);

                case MapObjectOrientationEnum.Top: return new Vector3(
                    (float)(point.X) * _MapGenerationScript.CellSize + _MapGenerationScript.CellSize / 2.0f
                    ,
                    _MapGenerationScript.WallHeight / 2.0f
                    ,
                    (float)(point.Y) * _MapGenerationScript.CellSize);

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

        public Vector3 GetWallCentralSurfaceLocation(MapPoint point, MapObjectOrientationEnum wallOrientation)
        {
            switch (wallOrientation)
            {
                case MapObjectOrientationEnum.Left:
                    return new Vector3(
                    (float)(point.X) * _MapGenerationScript.CellSize + _MapGenerationScript.WallWidth / 2.0f
                    ,
                    _MapGenerationScript.WallHeight / 2.0f
                    ,
                    (float)(point.Y) * _MapGenerationScript.CellSize + _MapGenerationScript.CellSize / 2.0f);

                case MapObjectOrientationEnum.Bottom:
                    return new Vector3(
                    (float)(point.X) * _MapGenerationScript.CellSize + _MapGenerationScript.CellSize / 2.0f
                    ,
                    _MapGenerationScript.WallHeight / 2.0f
                    ,
                    (float)(point.Y + 1) * _MapGenerationScript.CellSize - _MapGenerationScript.WallWidth / 2.0f);

                case MapObjectOrientationEnum.Right:
                    return new Vector3(
                    (float)(point.X + 1) * _MapGenerationScript.CellSize - _MapGenerationScript.WallWidth / 2.0f
                    ,
                    _MapGenerationScript.WallHeight / 2.0f
                    ,
                    (float)(point.Y) * _MapGenerationScript.CellSize + _MapGenerationScript.CellSize / 2.0f);

                case MapObjectOrientationEnum.Top:
                    return new Vector3(
                    (float)(point.X) * _MapGenerationScript.CellSize + _MapGenerationScript.CellSize / 2.0f
                    ,
                    _MapGenerationScript.WallHeight / 2.0f
                    ,
                    (float)(point.Y) * _MapGenerationScript.CellSize + _MapGenerationScript.WallWidth / 2.0f);

                default: throw new ArgumentException();
            }
        }

        public Vector3 GetPillarLocation(MapPoint point, MapObjectOrientationEnum pillarOrientation)
        {
            switch (pillarOrientation)
            {
                case MapObjectOrientationEnum.TopLeft:
                    return new Vector3(
                    (float)(point.X) * _MapGenerationScript.CellSize
                    ,
                    _MapGenerationScript.WallHeight / 2.0f
                    ,
                    (float)(point.Y) * _MapGenerationScript.CellSize);

                case MapObjectOrientationEnum.TopRight:
                    return new Vector3(
                    (float)(point.X + 1) * _MapGenerationScript.CellSize
                    ,
                    _MapGenerationScript.WallHeight / 2.0f
                    ,
                    (float)(point.Y) * _MapGenerationScript.CellSize);

                case MapObjectOrientationEnum.BottomLeft:
                    return new Vector3(
                    (float)(point.X) * _MapGenerationScript.CellSize
                    ,
                    _MapGenerationScript.WallHeight / 2.0f
                    ,
                    (float)(point.Y + 1) * _MapGenerationScript.CellSize);

                case MapObjectOrientationEnum.BottomRight:
                    return new Vector3(
                    (float)(point.X + 1) * _MapGenerationScript.CellSize
                    ,
                    _MapGenerationScript.WallHeight / 2.0f
                    ,
                    (float)(point.Y + 1) * _MapGenerationScript.CellSize);

                default: throw new ArgumentException();
            }
        }

        public Vector3 GetCellCentralLocation(MapPoint point) => new Vector3(
            (float)(point.X) * _MapGenerationScript.CellSize + _MapGenerationScript.CellSize / 2.0f
            ,
            _MapGenerationScript.WallHeight / 2.0f
            ,
            (float)(point.Y) * _MapGenerationScript.CellSize + _MapGenerationScript.CellSize / 2.0f);

        public Vector3 GetFloorLocation(MapPoint point) => new Vector3(
            (float)(point.X) * _MapGenerationScript.CellSize + _MapGenerationScript.CellSize / 2.0f
            ,
            0
            ,
            (float)point.Y * _MapGenerationScript.CellSize + _MapGenerationScript.CellSize / 2.0f);

        public Vector3 GetRoofLocation(MapPoint point) => new Vector3(
            (float)(point.X) * _MapGenerationScript.CellSize + _MapGenerationScript.CellSize / 2.0f
            ,
            _MapGenerationScript.WallHeight
            ,
            (float)(point.Y) * _MapGenerationScript.CellSize + _MapGenerationScript.CellSize / 2.0f);

        public Vector3 GetFloorCentralSurfaceLocation(MapPoint point) => new Vector3(
            (float)(point.X) * _MapGenerationScript.CellSize + _MapGenerationScript.CellSize / 2.0f
            ,
            _MapGenerationScript.FloorHeight / 2.0f
            ,
            (float)(point.Y) * _MapGenerationScript.CellSize + _MapGenerationScript.CellSize / 2.0f);

        public Vector3 GetRoofCentralSurfaceLocation(MapPoint point) => new Vector3(
            (float)(point.X) * _MapGenerationScript.CellSize + _MapGenerationScript.CellSize / 2.0f
            ,
            _MapGenerationScript.WallHeight - _MapGenerationScript.RoofHeight / 2.0f
            ,
            (float)(point.Y) * _MapGenerationScript.CellSize + _MapGenerationScript.CellSize / 2.0f);
    }
}
