using Assets.Project.Code.Scripts.Agents.Help;
using MapGenearionLibrary.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Project.Code.Scripts.Agents.AgentBehaviour
{
    public class AgentMovementScript : AAgentScript
    {
        public enum AgentMovementState
        {
            Idle = 0,
            Moving = 1
        }

        [SerializeField]
        public float Speed = 2f;
        public AgentMovementState MovementState { get; private set; } = AgentMovementState.Idle;

        private AgentNavigationPathHandler _Navigation { get; set; }
        private Transform _AgentTransformComponent { get; set; }

        protected override void _Start()
        {
            _Navigation = new AgentNavigationPathHandler(BaseScript.MapHandlerScript);
            _AgentTransformComponent = GetComponent<Transform>();
        }

        protected override void _Update()
        {
            _Navigation.SetCurrentLocation(_AgentTransformComponent.localPosition);
            if (BaseScript.AgentManagerScript.Params is not null)
            {
                _Navigation.PathCollisionDetectionDistance = BaseScript.AgentManagerScript.Params.PathCollisionDetectionDistance;
            }

            switch (MovementState)
            {
                case AgentMovementState.Idle:
                    MovementState = _HandleIdleState();
                    break;

                case AgentMovementState.Moving:
                    MovementState = _HandleMovementState();
                    break;
            }
        }

        private AgentMovementState _HandleIdleState()
        {
            _Navigation.SetTartgetMapPoint(BaseScript.MapHandlerScript.GetRandomMapPoint());

            return AgentMovementState.Moving;
        }

        private AgentMovementState _HandleMovementState()
        {
            if (_Navigation.IsTargetLocationAchived())
            {
                return AgentMovementState.Idle;
            }
            else
            {
                Vector3 direction = (_Navigation.CurrentTargetLocation - _AgentTransformComponent.localPosition);
                direction.y = 0;
                direction.Normalize();

                _AgentTransformComponent.localRotation = Quaternion.LookRotation(direction);

                Vector3 movement = (new Vector3(0, 0, 1.0f)) * Speed * Time.deltaTime;
                //Vector3.ClampMagnitude(movement, Speed);

                _AgentTransformComponent.Translate(movement);

                RaycastHit raycastHit;

                if (Physics.Raycast(_AgentTransformComponent.position, direction, out raycastHit, 10.0f))
                {
                    _Navigation.DistanceToObstacle = raycastHit.distance;
                }

                _Navigation.InvokeNavigation();

                return AgentMovementState.Moving;
            }
        }
    }
}
