using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Code.Scripts.Map
{
    public class MapParamsScript : MonoBehaviour
    {
        public int Width = 10;
        public int Height = 10;
        public int MaxLayerCount = -1;
        public int MinRoomWidth = -1;
        public int MinRoomHeight = -1;
        public int MaxDoorsCount = -1;
        public bool IsRandomDoorsCount = true;
        public float CellSize = 1.0f;
        public float WallWidth = 0.1f;
        public float WallHeight = 1.0f;
        public float FloorHeight = 0.1f;
        public float RoofHeight = 0.1f;
    }
}
