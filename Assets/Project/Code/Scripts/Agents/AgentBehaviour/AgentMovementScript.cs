using Assets.Project.Code.Scripts.Agents.Help;
using MapGenearionLibrary.Base;
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
            Moving = 1,
            Rotation = 2
        }

        [SerializeField]
        public float MovementSpeed = 2f;
        [SerializeField]
        public float RotationSpeed = 180f;
        public AgentMovementState MovementState { get; private set; } = AgentMovementState.Idle;

        private AgentNavigationPathHandler _Navigation { get; set; }
        private Transform _AgentTransformComponent { get; set; }

        protected override void _Start()
        {
            _Navigation = new AgentNavigationPathHandler(BaseScript.AgentManagerScript , BaseScript.MapHandlerScript, BaseScript);
            _AgentTransformComponent = GetComponent<Transform>();
        }

        protected override void _Update()
        {
            _Navigation.SetCurrentLocation(_AgentTransformComponent.position);

            switch (MovementState)
            {
                case AgentMovementState.Idle:
                    MovementState = _HandleIdleState();
                    break;

                case AgentMovementState.Moving:
                    MovementState = _HandleMovementState();
                    break;

                case AgentMovementState.Rotation:
                    MovementState = _HandleRotationState();
                    break;
            }
        }

        private AgentMovementState _HandleIdleState()
        {
            //_Navigation.SetTartgetMapPoint(BaseScript.MapHandlerScript.GetNearbyRoomPoint(_Navigation.CurrentMapPoint));
            _Navigation.SetTartgetMapPoint(BaseScript.MapHandlerScript.GetRandomNoneObstacleMapPoint());

            return AgentMovementState.Rotation;
        }

        private AgentMovementState _HandleMovementState()
        {
            if (_Navigation.IsTargetLocationAchived())
            {
                return AgentMovementState.Idle;
            }
            else
            {
                var prevTargetLocation = _Navigation.CurrentTargetLocation;

                Vector3 direction = (_Navigation.CurrentTargetLocation - _AgentTransformComponent.position);
                direction.y = 0;
                direction.Normalize();

                _AgentTransformComponent.localRotation = Quaternion.LookRotation(direction);

                RaycastHit raycastHit;

                if (Physics.Raycast(_AgentTransformComponent.position, direction, out raycastHit, 10.0f))
                {
                    _Navigation.DistanceToObstacle = raycastHit.distance;
                }
                else
                {
                    _Navigation.DistanceToObstacle = float.MaxValue;
                }

                _Navigation.InvokeNavigation();

                if (_Navigation.IsNoPath)
                {
                    return AgentMovementState.Idle;
                }

                if (!_Navigation.IsMovementBlocked)
                {
                    Vector3 movement = (new Vector3(0, 0, 1.0f)) * MovementSpeed * Time.deltaTime;

                    _AgentTransformComponent.Translate(movement);
                }

                if (Vector3.Distance(prevTargetLocation, _Navigation.CurrentTargetLocation) > 0.1f)
                {
                    return AgentMovementState.Rotation;
                }
                else
                {
                    return AgentMovementState.Moving;
                }
            }
        }

        private AgentMovementState _HandleRotationState()
        {
            Vector3 direction = (_Navigation.CurrentTargetLocation - _AgentTransformComponent.position);
            direction.y = 0;
            direction.Normalize();

            var targetRotation = Quaternion.LookRotation(direction);

            _AgentTransformComponent.localRotation = Quaternion.RotateTowards(
                _AgentTransformComponent.localRotation,
                targetRotation,
                RotationSpeed * Time.deltaTime);

            if (Quaternion.Angle(_AgentTransformComponent.localRotation, targetRotation) < 5.0f
                || Vector3.Distance(_Navigation.CurrentTargetLocation, _AgentTransformComponent.position) < BaseScript.AgentManagerScript.Params.PathInternalPointAchiveDistance)
            {
                return AgentMovementState.Moving;
            }
            else
            {
                return AgentMovementState.Rotation;
            }
        }

        public override void Dispose()
        {
            _Navigation.Dispose();
        }
    }
}
