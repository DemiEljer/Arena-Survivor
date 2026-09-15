using Assets.Project.Code.Scripts.Agents.Fabrics;
using Assets.Project.Code.Scripts.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Code.Scripts.Agents.Generators
{
    public abstract class AAgentGeneratorScript : MonoBehaviour
    {
        protected AgentManagerScript _AgentManager { get; set; }

        public event Action<GameObject, AgentBaseScript> AgentHasBeenGeneratedEvent;

        private void Start()
        {
            _AgentManager = GetComponent<AgentManagerScript>();

            if (_AgentManager is not null)
            {
                _AgentManager.AppendGenerator(this);
            }

            _Start();
        }

        public void Generate(AgentManagerScript agentManager)
        {
            if (agentManager is not null
                && agentManager.MapHandlerScript is not null)
            {
                try
                {
                    _Generate(agentManager);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        protected abstract void _Generate(AgentManagerScript agentManager);

        protected abstract void _Start();

        protected void _RegistrateFabric(AAgentFabricScript fabric)
        {
            if (fabric is not null)
            {
                fabric.ObjectHasBeenCreatedEvent += (gameObject) => _InvokeAgentGenerationEvent(gameObject);
            }
        }

        protected void _InvokeAgentGenerationEvent(GameObject agentGameObject)
        {
            var agentBaseScript = agentGameObject?.GetComponent<AgentBaseScript>();

            if (agentBaseScript is not null)
            {
                AgentHasBeenGeneratedEvent?.Invoke(agentGameObject, agentBaseScript);
            }
            else
            {
                Destroy(agentGameObject);
            }
        }
    }
}
