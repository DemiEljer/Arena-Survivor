using Assets.Project.Code.Scripts.Agents.Generators;
using Assets.Project.Code.Scripts.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Code.Scripts.Agents
{
    public class AgentManagerScript : MonoBehaviour
    {
        public GameObject MapOwner;
        public MapGenerationScript MapHandlerScript { get; private set; }

        private List<AAgentGeneratorScript> _AgentGenerators { get; } = new List<AAgentGeneratorScript>();
        private Dictionary<AgentBaseScript, GameObject> _Agents { get; } = new Dictionary<AgentBaseScript, GameObject>();

        void Start()
        {
            if (MapOwner is not null)
            {
                MapHandlerScript = MapOwner.GetComponent<MapGenerationScript>();

                if (MapHandlerScript is not null)
                {
                    MapHandlerScript.MapHasBeenGeneratedEvent += MapHasBeenGeneratedEventHandler;
                }
            }
        }

        private void Update()
        {
            
        }

        public void MapHasBeenGeneratedEventHandler(MapGenerationScript mapHandlerScript)
        {
            foreach (var generator in _AgentGenerators)
            {
                generator.Generate(this);
            }
        }

        public void AppendGenerator(AAgentGeneratorScript generator)
        {
            if (generator is not null)
            {
                _AgentGenerators.Add(generator);

                generator.AgentHasBeenGeneratedEvent += AgentHasBeenGenerated;
            }
        }

        public void AgentHasBeenGenerated(GameObject agentGameObject, AgentBaseScript agentBaseScript)
        {
            agentBaseScript.ObjectHasBeenDisposedEvent += AgentHasBeenDisposed;

            agentBaseScript.SetMapHandlerScript(MapHandlerScript);

            _Agents.Add(agentBaseScript, agentGameObject);
        }

        public void AgentHasBeenDisposed(AgentBaseScript agentBaseScript)
        {
            if (_Agents.ContainsKey(agentBaseScript))
            {
                _Agents.Remove(agentBaseScript);
            }
        }

        public void DeleteAllAgents()
        {
            while (_Agents.Count > 0)
            {
                Destroy(_Agents.ElementAt(0).Value);
            }
        }
    }
}
