using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Code.Scripts.Map.Fabrics
{
    public class MapLightObjectFabricScript : AMapObjectFabricScript
    {
        public GameObject LightPointPrefab;

        public GameObject CreateLightPoint(Vector3 location) => _CreateInstance(LightPointPrefab, location, Quaternion.identity, new Vector3(1, 1, 1));
    }
}
