using Assets.Project.Code.Scripts.Map.Fabrics;
using Assets.Project.Code.Scripts.Map.Help;
using MapGenearionLibrary.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Project.Code.Scripts.Map.Generators
{
    public class MapLightObjectGenerationScript : AMapObjectGenerationScript
    {
        public bool DoesGenerateLightPoints = false;

        private MapLightObjectFabricScript _ObjectsFabric { get; set; }

        private Random _Rnd { get; } = new Random();

        private MapCellDensityCalculation _PointLightDensity { get; set; } 

        protected override void _Start()
        {
            _ObjectsFabric = GetComponent<MapLightObjectFabricScript>();

            _PointLightDensity = new MapCellDensityCalculation(_MapGenerationScript.Map, 0.5f);
        }

        protected override void _Generate(MapGenerationScript mapGenerationScript)
        {
            if (_ObjectsFabric is null)
            {
                return;
            }

            void _CreateLightPointInRoom(MapRoom room)
            {
                MapPoint randomRoomPoint = new MapPoint(_Rnd.Next(room.StartX, room.StartX + room.Width), _Rnd.Next(room.StartY, room.StartY + room.Height));

                var newLightObject = _ObjectsFabric.CreateLightPoint(mapGenerationScript.ObjectLocations.GetCellCentralLocation(randomRoomPoint));
                newLightObject.name = "PointLight";

                mapGenerationScript.AppendObjectAsChild(newLightObject);
            }

            void _CreateLightPoint(MapPoint lightLocation)
            {
                var newLightObject = _ObjectsFabric.CreateLightPoint(mapGenerationScript.ObjectLocations.GetCellCentralLocation(lightLocation));
                newLightObject.name = "PointLight";

                mapGenerationScript.AppendObjectAsChild(newLightObject);
            }

            foreach (var room in mapGenerationScript.MapNavigation.Rooms)
            {
                if (DoesGenerateLightPoints)
                {
                    //_CreateLightPointInRoom(room);
                }
            }

            while (!_PointLightDensity.IsFullfilled)
            {
                var vaicantPoint = _PointLightDensity.GetMinValueCells().First();

                if (vaicantPoint is not null)
                {
                    _PointLightDensity.Set(vaicantPoint);

                    _CreateLightPoint(vaicantPoint);
                }
                else
                {
                    break;
                }
            }
        }
    }
}
