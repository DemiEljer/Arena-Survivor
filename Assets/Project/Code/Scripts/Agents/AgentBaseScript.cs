using Assets.Project.Code.Scripts.Map;
using Assets.Project.Code.Standard.Collections;
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
        private static CollectionUniqIndexer _Indexer { get; } = new();

        public ulong ObjectIndex { get; }

        public MapGenerationScript MapHandlerScript { get; private set; }
        public AgentManagerScript AgentManagerScript { get; private set; }

        public Action<AgentBaseScript> ObjectHasBeenDisposedEvent;

        public AgentBaseScript()
        {
            ObjectIndex = _Indexer.GetNextIndex();
        }

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
