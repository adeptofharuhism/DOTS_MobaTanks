﻿using Assets.CodeBase.Combat.Teams;
using Assets.CodeBase.GameStates;
using Assets.CodeBase.Vehicles;
using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace Assets.CodeBase.Shop
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    [UpdateBefore(typeof(InitializeVehicleSystem))]
    public partial struct ServerShopActivatorInitializationSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NewVehicleTag>();
            state.RequireForUpdate<InGameState>();
            state.RequireForUpdate<ShopActivationSettings>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            ShopActivationSettings shopActivationSettings = SystemAPI.GetSingleton<ShopActivationSettings>();

            foreach (var (team, vehicle)
                in SystemAPI.Query<UnitTeam>()
                .WithAll<NewVehicleTag>()
                .WithEntityAccess()) {

                ecb.AddComponent<SquaredDistanceToShop>(vehicle);
                ecb.AddComponent(vehicle, new ShopIsActive { Value = false });
                ecb.AddComponent(vehicle, new SquaredShopActivationDistance { Value = shopActivationSettings.SquaredActivationDistance });
                ecb.AddComponent(vehicle, new ShopPosition {
                    Value = team.Value == TeamType.Blue
                        ? shopActivationSettings.BlueShopPosition
                        : shopActivationSettings.OrangeShopPosition
                });
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateBefore(typeof(InitializeVehicleLocalOwnershipSystem))]
    public partial struct ClientShopActivatorInitializationSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
            state.RequireForUpdate<ShopActivationSettings>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            ShopActivationSettings shopActivationSettings = SystemAPI.GetSingleton<ShopActivationSettings>();

            foreach (var (team, vehicle)
                in SystemAPI.Query<UnitTeam>()
                .WithAll<GhostOwnerIsLocal, NotOwnerVehicleTag>()
                .WithEntityAccess()) {

                ecb.AddComponent<SquaredDistanceToShop>(vehicle);
                ecb.AddComponent(vehicle, new ShopIsActive { Value = false });
                ecb.AddComponent(vehicle, new SquaredShopActivationDistance { Value = shopActivationSettings.SquaredActivationDistance });
                ecb.AddComponent(vehicle, new ShopPosition {
                    Value = team.Value == TeamType.Blue
                        ? shopActivationSettings.BlueShopPosition
                        : shopActivationSettings.OrangeShopPosition
                });
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [UpdateInGroup(typeof(ShopSystemGroup))]
    [UpdateBefore(typeof(CheckIfShopIsActiveSystem))]
    public partial struct CalculateSquaredDistanceToShopSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            foreach (var (distanceToShop, transform, shopPosition)
                in SystemAPI.Query<RefRW<SquaredDistanceToShop>, LocalTransform, ShopPosition>())

                distanceToShop.ValueRW.Value = math.distancesq(shopPosition.Value, transform.Position);
        }
    }

    [UpdateInGroup(typeof(ShopSystemGroup))]
    [UpdateAfter(typeof(CalculateSquaredDistanceToShopSystem))]
    public partial struct CheckIfShopIsActiveSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            foreach (var (distanceToShop, activationDistance, activeFlag)
                in SystemAPI.Query<SquaredDistanceToShop, SquaredShopActivationDistance, RefRW<ShopIsActive>>())

                activeFlag.ValueRW.Value = activationDistance.Value > distanceToShop.Value;
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(ShopSystemGroup))]
    [UpdateAfter(typeof(CheckIfShopIsActiveSystem))]
    public partial class ShopAvailabilityCheckSystem : SystemBase
    {
        public Action<bool> OnShopAvailabilityChanged;

        private bool _shopIsActive;
        public bool ShopIsActive {
            get => _shopIsActive;
            private set {
                if (_shopIsActive != value)
                    OnShopAvailabilityChanged?.Invoke(value);

                _shopIsActive = value;
            }
        }

        protected override void OnCreate() {
            ShopIsActive = false;

            RequireForUpdate<InGameState>();
        }

        protected override void OnUpdate() {
            bool hasShopFlagCarrierInWorld = SystemAPI.TryGetSingleton(out ShopIsActive activeFlag);

            if (hasShopFlagCarrierInWorld)
                ShopIsActive = activeFlag.Value;
            else
                ShopIsActive = false;
        }
    }
}