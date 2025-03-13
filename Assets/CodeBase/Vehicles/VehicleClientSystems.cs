using Assets.CodeBase.Effects.Following;
using Assets.CodeBase.Inventory.TargetingRange;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
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
        private const float TargetZoneMultiplier = 2.1f;

        private float[] _currentInventoryRanges;

        private int _farRangeIndex;
        private int _closeRangeIndex;
        private Entity _farRangeDecal;
        private Entity _closeRangeDecal;

        private bool _shouldUpdateScales = false;

        protected override void OnCreate() {
            _currentInventoryRanges = new float[InventorySize];

            RequireForUpdate<TargetingDecalPrefabs>();
            RequireForUpdate<UpdateTargetRangeElement>();
        }

        protected override void OnUpdate() {
            DynamicBuffer<UpdateTargetRangeElement> updateTargetRangeBuffer =
                SystemAPI.GetSingletonBuffer<UpdateTargetRangeElement>();

            if (updateTargetRangeBuffer.Length > 0) {
                //Set ranges inside local array
                foreach (UpdateTargetRangeElement item in updateTargetRangeBuffer)
                    _currentInventoryRanges[item.SlotId] = item.TargetRange;

                updateTargetRangeBuffer.Clear();

                //Update far and close ranges
                _farRangeIndex = _closeRangeIndex = 0;
                for (int i = 1; i < _currentInventoryRanges.Length; i++) {
                    _farRangeIndex =
                        _currentInventoryRanges[i] > _currentInventoryRanges[_farRangeIndex]
                            ? i
                            : _farRangeIndex;
                    _closeRangeIndex =
                        (_currentInventoryRanges[i] < _currentInventoryRanges[_closeRangeIndex] &&
                        _currentInventoryRanges[i] > 1)
                            ? i
                            : _closeRangeIndex;
                }

                _shouldUpdateScales = true;
            }

            //Instantiate decals and link them to vehicle, if new vehicle spawns
            foreach(var (linkedEntityGroup, vehicle)
                in SystemAPI.Query<DynamicBuffer<LinkedEntityGroup>>()
                .WithAll<NewVehicleTag, GhostOwnerIsLocal>()
                .WithEntityAccess()) {

                TargetingDecalPrefabs targetingRangePrefab = SystemAPI.GetSingleton<TargetingDecalPrefabs>();

                _farRangeDecal = EntityManager.Instantiate(targetingRangePrefab.FarRangeDecalPrefab);
                _closeRangeDecal = EntityManager.Instantiate(targetingRangePrefab.CloseRangeDecalPrefab);

                linkedEntityGroup.Add(_farRangeDecal);
                linkedEntityGroup.Add(_closeRangeDecal);

                EntityManager.SetComponentData(_farRangeDecal, new FollowTarget { Value = vehicle });
                EntityManager.SetComponentData(_closeRangeDecal, new FollowTarget { Value = vehicle });

                _shouldUpdateScales = true;
            }

            //Update existing decals, if they exist
            if (_shouldUpdateScales) {
                ComponentLookup<LocalTransform> transformLookup = SystemAPI.GetComponentLookup<LocalTransform>();
                bool decalExists =
                    transformLookup.TryGetComponent(_farRangeDecal, out LocalTransform farRangeDecalTransform);

                if (decalExists) {
                    LocalTransform closeRangeDecalTransform = transformLookup[_closeRangeDecal];

                    farRangeDecalTransform.Scale = _currentInventoryRanges[_farRangeIndex] * TargetZoneMultiplier;
                    closeRangeDecalTransform.Scale = _currentInventoryRanges[_closeRangeIndex] * TargetZoneMultiplier;

                    transformLookup[_farRangeDecal] = farRangeDecalTransform;
                    transformLookup[_closeRangeDecal] = closeRangeDecalTransform;
                }

                _shouldUpdateScales = false;
            }
        }
    }
}
