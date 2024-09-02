using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

namespace Assets.CodeBase.Vehicles
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(VehicleSystemGroup))]
    [UpdateBefore(typeof(VehicleMovementInputSystem))]
    public partial struct InitializeVehicleLocalOwnershipSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (Entity vehicle
                in SystemAPI.QueryBuilder()
                .WithAll<GhostOwnerIsLocal, NotOwnerVehicleTag>()
                .Build().ToEntityArray(Allocator.Temp)) {

                ecb.AddComponent<OwnerVehicleTag>(vehicle);
                ecb.RemoveComponent<NotOwnerVehicleTag>(vehicle);

                ecb.SetComponent(vehicle, new VehicleMovementInput { Value = new float2(0, 0) });
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(VehicleSystemGroup))]
    [UpdateAfter(typeof(InitializeVehicleLocalOwnershipSystem))]
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
