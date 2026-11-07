using Assets.Project.Code.Scripts.Map.Fabrics;
using Assets.Project.Code.Standard.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Code.Scripts.Agents.Fabrics
{
    public class AgentEnemyFabricScript : AAgentFabricScript
    {
        public GameObject[] DefauleMobes;

        private CollectionIndexCyclicalIterator _DefaultMobesIterator { get; } = new();

        public GameObject CreateDefaultMobe(Vector3 location) => _CreateInstance(DefauleMobes, _DefaultMobesIterator, location, Quaternion.identity);
    }
}
