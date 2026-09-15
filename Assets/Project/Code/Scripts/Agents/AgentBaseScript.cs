using Assets.Project.Code.Scripts.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Project.Code.Scripts.Agents
{
    public class AgentBaseScript : MonoBehaviour
    {
        public MapGenerationScript MapHandlerScript { get; private set; }
        public AgentManagerScript AgentManagerScript { get; private set; }

        public Action<AgentBaseScript> ObjectHasBeenDisposedEvent;

        void Start()
        {
            
        }

        void Update()
        {

        }

        void OnDestroy()
        {
            Dispose();
        }

        public void SetMapHandlerScript(MapGenerationScript mapHandlerScript)
        {
            MapHandlerScript = mapHandlerScript;
        }

        public void SetAgentManagerScript(AgentManagerScript agentManagerScript)
        {
            AgentManagerScript = agentManagerScript;
        }

        public void Dispose()
        {
            ObjectHasBeenDisposedEvent?.Invoke(this);
        }
    }
}
