using Assets.Project.Code.Standard.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Code.Scripts.Map.Fabrics
{
    public class MapStaticObjectFabricScript : AMapObjectFabricScript
    {
        public GameObject[] CentralCellObjectsPrefabs = Array.Empty<GameObject>();

        public GameObject[] WallObjectsPrefabs = Array.Empty<GameObject>();

        private CollectionIndexCyclicalIterator _CentralCellObjectsIterator { get; } = new();
        private CollectionIndexCyclicalIterator _WallObjectsIterator { get; } = new();

        public GameObject CreateCentralCellObject(Vector3 location, Quaternion quaternion) => _CreateInstance(CentralCellObjectsPrefabs, _CentralCellObjectsIterator, location, quaternion);
        public GameObject CreateWallObject(Vector3 location, Quaternion quaternion) => _CreateInstance(WallObjectsPrefabs, _WallObjectsIterator, location, quaternion);
    }
}
