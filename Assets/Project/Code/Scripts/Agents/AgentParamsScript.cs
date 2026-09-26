using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Code.Scripts.Agents
{
    public class AgentParamsScript : MonoBehaviour
    {
        public float PathCollisionDetectionDistance = 0.2f;
        public float PathInternalPointAchiveDistance = 0.2f;
        public float PathTargetPointAchiveDistance = 0.4f;
        public int PathSearchingDepth = 5;
    }
}
