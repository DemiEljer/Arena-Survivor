using Assets.Project.Code.Scripts.Agents.Fabrics;
using Assets.Project.Code.Scripts.Map;
using Assets.Project.Code.Scripts.Map.Generators;
using MapGenearionLibrary.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Project.Code.Scripts.Agents.Generators
{
    public class AgentEnemyGeneratorScript : AAgentGeneratorScript
    {
        public bool DoesGenerateEnemies = false;
        public int EnemyCount = 5;

        private AgentEnemyFabricScript _EnemyFabric { get; set; }

        protected override void _Start()
        {
            _EnemyFabric = GetComponent<AgentEnemyFabricScript>();

            _RegistrateFabric(_EnemyFabric);
        }

        protected override void _Generate(AgentManagerScript agentManager)
        {
            if (_EnemyFabric is not null)
            {
                var rnd = MapGenerationScript.Rnd;
                var mapHandler = agentManager.MapHandlerScript;

                if (DoesGenerateEnemies)
                {
                    void _CreateEnemy()
                    {
                        var randomMapPoint = new MapPoint(rnd.Next(0, mapHandler.Map.Width), rnd.Next(0, mapHandler.Map.Height));

                        var newEnemyObject = _EnemyFabric.CreateDefaultEnemy(mapHandler.ObjectLocations.GetCellCentralLocation(randomMapPoint));
                        newEnemyObject.name = "Enemy";
                    }

                    for (int enemyIndex = 0; enemyIndex < EnemyCount; enemyIndex++)
                    {
                        _CreateEnemy();
                    }
                }
            }
        }
    }
}
