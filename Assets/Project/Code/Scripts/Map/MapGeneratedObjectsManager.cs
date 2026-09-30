using MapGenearionLibrary.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Code.Scripts.Map
{
    public class MapGeneratedObjectsManager
    {
        public class GeneratedObjectInfo
        {
            public GameObject GameObject { get; }

            public MapPoint Location { get; }

            public MapObjectOrientationEnum Orientation { get; }

            public bool IsCentral => Orientation == MapObjectOrientationEnum.Center;

            public bool IsOnWall => (int)Orientation < 4;

            public bool IsInCorner => (int)Orientation >= 4 && (int)Orientation < 8;

            public GeneratedObjectInfo(MapPoint location, MapObjectOrientationEnum orientation, GameObject gameObject = null)
            {
                Location = location;
                Orientation = orientation;
                GameObject = gameObject;
            }
        }

        private List<GeneratedObjectInfo> _GeneratedObjects { get; } = new();

        public IEnumerable<GeneratedObjectInfo> GetObjectsInfo()
        {
            foreach (var objectInfo in _GeneratedObjects)
            {
                yield return objectInfo;
            }
        }

        public void AppendObject(MapPoint location, MapObjectOrientationEnum orientation, GameObject gameObject = null) =>
            AppendObject(new GeneratedObjectInfo(location, orientation, gameObject));

        public void AppendObject(GeneratedObjectInfo objectInfo)
        {
            if (objectInfo is not null)
            {
                _GeneratedObjects.Add(objectInfo);
            }
        }

        public IEnumerable<GeneratedObjectInfo> GetObjectsInMapCell(MapPoint point) => _GeneratedObjects.Where(objectInfo => objectInfo.Location.AreEqual(point));
    }
}
