using Assets.Project.Code.Scripts.Agents;
using Assets.Project.Code.Scripts.Agents.Fabrics;
using Assets.Project.Code.Scripts.Map.Fabrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Code.Scripts.Map.Generators
{
    public abstract class AMapObjectGenerationScript : MonoBehaviour
    {
        protected MapGenerationScript _MapGenerationScript { get; set; }

        public event Action<GameObject> ObjectHasBeenGeneratedEvent;

        private void Start()
        {
            _MapGenerationScript = GetComponent<MapGenerationScript>();
            
            if (_MapGenerationScript is not null)
            {
                _MapGenerationScript.AppendGenerator(this);
            }

            _Start();
        }

        public void Generate(MapGenerationScript mapGenerationScript)
        {
            if (mapGenerationScript is not null && mapGenerationScript.Params is not null)
            {
                try
                {
                    _Generate(mapGenerationScript);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        protected abstract void _Generate(MapGenerationScript mapGenerationScript);

        protected abstract void _Start();

        protected void _RegistrateFabric(AMapObjectFabricScript fabric)
        {
            if (fabric is not null)
            {
                fabric.ObjectHasBeenCreatedEvent += (gameObject) => _InvokeAgentGenerationEvent(gameObject);
            }
        }

        protected void _InvokeAgentGenerationEvent(GameObject gameObject)
        {
            ObjectHasBeenGeneratedEvent?.Invoke(gameObject);
        }
    }
}
