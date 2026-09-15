using Assets.Project.Code.Scripts.Agents.Help;
using MapGenearionLibrary.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
                Vector3 movement = (_Navigation.CurrentTargetLocation - _AgentTransformComponent.localPosition);
                movement.y = 0;
                movement.Normalize();
                movement *= Speed * Time.deltaTime;
                Vector3.ClampMagnitude(movement, Speed);

                _AgentTransformComponent.Translate(movement);

                _Navigation.InvokeNavigation();

                return AgentMovementState.Moving;
            }
        }
    }
}
