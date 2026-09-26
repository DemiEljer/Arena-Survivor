using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Code.Scripts.Map.Fabrics
{
    public class MapComplexObstaclesFabricScript : AMapObjectFabricScript
    {
        public const float FloorAndRoofHeight = 0.1f;
        public const float FloorAndRoofSize = 1.1f;
        public const float WallWidth = 0.1f;
        public const float WallHeight = 1.0f;
        public const float WallLength = 1.0f;

        public GameObject WallCornerRight;
        public GameObject WallCornerLeft;
        public GameObject WallFlatLeft;
        public GameObject WallFlatRight;
        public GameObject Floor;
        public GameObject RoofNoWalls;
        public GameObject RoofFlat;
        public GameObject RoofCornerLeft;
        public GameObject RoofCornerRight;
        public GameObject PillarOut;
        public GameObject PillarIn;
        public GameObject Acrh;

        public GameObject CreateWallCornerRight(Vector3 location, Quaternion quaternion, Vector3 scale) => _CreateInstance(WallCornerLeft, location, quaternion, _GetWallScale(scale));
        public GameObject CreateWallCornerLeft(Vector3 location, Quaternion quaternion, Vector3 scale) => _CreateInstance(WallCornerRight, location, quaternion, _GetWallScale(scale));
        public GameObject CreateWallFlatLeft(Vector3 location, Quaternion quaternion, Vector3 scale) => _CreateInstance(WallFlatRight, location, quaternion, _GetWallScale(scale));
        public GameObject CreateWallFlatRight(Vector3 location, Quaternion quaternion, Vector3 scale) => _CreateInstance(WallFlatLeft, location, quaternion, _GetWallScale(scale));
        public GameObject CreateFloor(Vector3 location, Quaternion quaternion, Vector3 scale) => _CreateInstance(Floor, location, quaternion, _GetFloorScale(scale));
        public GameObject CreateRoofNoWalls(Vector3 location, Quaternion quaternion, Vector3 scale) => _CreateInstance(RoofNoWalls, location, quaternion, _GetRoofScale(scale));
        public GameObject CreateRoofFlat(Vector3 location, Quaternion quaternion, Vector3 scale) => _CreateInstance(RoofFlat, location, quaternion, _GetRoofScale(scale));
        public GameObject CreateRoofCornerLeft(Vector3 location, Quaternion quaternion, Vector3 scale) => _CreateInstance(RoofCornerLeft, location, quaternion, _GetRoofScale(scale));
        public GameObject CreateRoofCornerRight(Vector3 location, Quaternion quaternion, Vector3 scale) => _CreateInstance(RoofCornerRight, location, quaternion, _GetRoofScale(scale));
        public GameObject CreatePillarOut(Vector3 location, Quaternion quaternion, Vector3 scale) => _CreateInstance(PillarOut, location, quaternion, _GetPillarScale(scale));
        public GameObject CreatePillarIn(Vector3 location, Quaternion quaternion, Vector3 scale) => _CreateInstance(PillarIn, location, quaternion, _GetPillarScale(scale));
        public GameObject CreateAcrh(Vector3 location, Quaternion quaternion, Vector3 scale) => _CreateInstance(Acrh, location, quaternion, _GetArchScale(scale));

        private Vector3 _GetWallScale(Vector3 scale) => new Vector3(
            scale.x / WallWidth
            ,
            scale.y / WallHeight
            ,
            scale.z / WallLength
        );

        private Vector3 _GetFloorScale(Vector3 scale) => new Vector3(
            scale.x / FloorAndRoofSize
            ,
            scale.y / FloorAndRoofHeight
            ,
            scale.z / FloorAndRoofSize
        );

        private Vector3 _GetRoofScale(Vector3 scale) => new Vector3(
            scale.x / FloorAndRoofSize
            ,
            scale.y / FloorAndRoofHeight
            ,
            scale.z / FloorAndRoofSize
        );

        private Vector3 _GetPillarScale(Vector3 scale) => new Vector3(
            scale.x / WallWidth
            ,
            scale.y / WallHeight
            ,
            scale.z / WallWidth
        );

        private Vector3 _GetArchScale(Vector3 scale) => new Vector3(
            scale.x / WallWidth
            ,
            scale.y / WallHeight
            ,
            scale.z / WallLength
        );
    }
}
