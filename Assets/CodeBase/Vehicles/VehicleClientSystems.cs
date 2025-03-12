using Assets.CodeBase.Inventory;
using Assets.CodeBase.Inventory.Items;
using System.Collections;
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

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(VehicleSystemGroup))]
    [UpdateAfter(typeof(VehicleMovementInputSystem))]
    public partial class VehicleTargetZoneUpdateSystem : SystemBase
    {
        private const int InventorySize = 7;
        private const int BasicWeaponRange = 50;
        private const float TargetZoneMultiplier = 1.11f;

        private int[] _currentInventoryRanges;

        protected override void OnCreate() {
            RequireForUpdate<UpdateTargetRangeElement>();
        }

        protected override void OnUpdate() {
            DynamicBuffer<UpdateTargetRangeElement> updateTargetRangeBuffer =
                SystemAPI.GetSingletonBuffer<UpdateTargetRangeElement>();

            if (updateTargetRangeBuffer.Length > 0) {
                foreach (var item in updateTargetRangeBuffer) {
                    UnityEngine.Debug.Log(item.TargetRange);
                    UnityEngine.Debug.Log(item.SlotId);
                }

                updateTargetRangeBuffer.Clear();
            }


            foreach (Entity vehicle
                in SystemAPI.QueryBuilder()
                .WithAll<NewVehicleTag, GhostOwnerIsLocal>()
                .Build().ToEntityArray(Allocator.Temp)) {


            }
        }

        private void UpdateItemRanges(int inventoryIndex, int itemIndex) {

        }
    }
}
