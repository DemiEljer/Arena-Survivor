using MapGenearionLibrary.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Project.Code.Scripts.Map.Help
{
    public class MapObstacleObjectAssociation
    {
        public MapGenerationScript MapGenerationScript { get; }

        private MapPoint _Location { get; set; } = new MapPoint(-1, -1);

        private Object _ObjectAssosiation { get; }

        public MapPoint Location 
        {
            get => _Location;
            set
            {
                if (MapGenerationScript is not null
                    && _Location is not null
                    && value is not null)
                {
                    if (!_Location.AreEqual(value))
                    {
                        MapGenerationScript.MapNavigation.DynamicObstacles.HandleSingle((obstacles) =>
                        {
                            obstacles.Release(_Location, _ObjectAssosiation);
                            _Location = value;
                            obstacles.ForceTake(_Location, _ObjectAssosiation);
                        });
                    }
                }
            }
        }

        public MapObstacleObjectAssociation(MapGenerationScript mapGenerationScript, Object objectAssosioation)
        {
            MapGenerationScript = mapGenerationScript;
            _ObjectAssosiation = objectAssosioation;
        }

        public MapObstacleObjectAssociation(MapGenerationScript mapGenerationScript, MapPoint location)
        {
            MapGenerationScript = mapGenerationScript;
            Location = location;
        }
    }
}
