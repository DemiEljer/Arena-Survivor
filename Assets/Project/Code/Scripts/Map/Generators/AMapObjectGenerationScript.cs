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
    }
}
