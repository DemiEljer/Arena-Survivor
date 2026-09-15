using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Code.Scripts.Agents
{
    public abstract class AAgentScript : MonoBehaviour
    {
        public AgentBaseScript BaseScript { get; private set; }

        void Start()
        {
            BaseScript = GetComponent<AgentBaseScript>();

            if (BaseScript is not null)
            {
                _Start();
            }
        }

        void Update()
        {
            if (BaseScript is not null)
            {
                _Update();
            }
        }

        protected virtual void _Start() { }

        protected virtual void _Update() { }
    }
}
