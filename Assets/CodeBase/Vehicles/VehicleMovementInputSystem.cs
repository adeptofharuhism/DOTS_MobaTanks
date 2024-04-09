using Assets.CodeBase.Vehicles;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

namespace Assets.CodeBase.Input
{
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    public partial class VehicleMovementInputSystem : SystemBase
    {
        private Controls _controls;

        private float2 _movementVector => _controls.Vehicle.Movement.ReadValue<Vector2>();

        protected override void OnCreate() {
            _controls = new Controls();

            RequireForUpdate<OwnerVehicleTag>();
        }

        protected override void OnStartRunning() {
            _controls.Enable();
        }

        protected override void OnStopRunning() {
            _controls.Disable();
        }

        protected override void OnUpdate() {
            Entity vehicle = SystemAPI.GetSingletonEntity<OwnerVehicleTag>();
            EntityManager.SetComponentData(vehicle, new VehicleMovementInput { Value = _movementVector });
        }
    }
}
