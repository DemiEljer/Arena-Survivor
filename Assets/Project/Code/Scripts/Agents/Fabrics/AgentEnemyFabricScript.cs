using Assets.Project.Code.Scripts.Map.Fabrics;
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
        public GameObject DefaultEnemyPrefab;

        public GameObject CreateDefaultEnemy(Vector3 location) => _CreateInstance(DefaultEnemyPrefab, location, Quaternion.identity);
    }
}
