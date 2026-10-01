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

        public MapPoint Location 
        {
            get => _Location;
            set
            {
                if (MapGenerationScript is not null
                    && _Location is not null
                    && value is not null)
                {
                    if (_Location.AreEqual(value))
                    {
                        MapGenerationScript.MapNavigation.Obstacles[_Location] = false;
                        _Location = value;
                        MapGenerationScript.MapNavigation.Obstacles[_Location] = true;
                    }
                }
            }
        }

        public MapObstacleObjectAssociation(MapGenerationScript mapGenerationScript)
        {
            MapGenerationScript = mapGenerationScript;
        }

        public MapObstacleObjectAssociation(MapGenerationScript mapGenerationScript, MapPoint location)
        {
            MapGenerationScript = mapGenerationScript;
            Location = location;
        }
    }
}
