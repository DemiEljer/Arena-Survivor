using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Code.Scripts.Map
{
    public abstract class AMapObjectGenerationScript : MonoBehaviour
    {
        private MapGenerationScript _MapGenerationScript { get; set; }

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
            if (mapGenerationScript is not null)
            {
                _Generate(mapGenerationScript);
            }
        }

        protected abstract void _Generate(MapGenerationScript mapGenerationScript);

        protected abstract void _Start();
    }
}
