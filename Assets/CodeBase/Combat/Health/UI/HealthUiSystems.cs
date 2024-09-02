using Assets.CodeBase.Infrastructure.PrefabInjection;
using Assets.CodeBase.Teams;
using Assets.CodeBase.Vehicles;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.CodeBase.Combat.Health.UI
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(HealthBarClientSystemGroup))]
    [UpdateBefore(typeof(HealthBarPlayerNameInitializationSystem))]
    public partial struct HealthBarInitializationSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<UIPrefabs>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (transform, healthBarOffset, entity)
                in SystemAPI.Query<LocalTransform, HealthBarOffset>()
                .WithAll<HealthBarInitializationTag>()
                .WithEntityAccess()) {

                bool isVehicle = SystemAPI.HasComponent<VehicleTag>(entity);

                GameObject newHealthBar = SpawnHealthBar(isVehicle, transform.Position, healthBarOffset.Value);

                InitializeHealthBar(ref state, ref ecb, entity, newHealthBar, isVehicle);
            }

            ecb.Playback(state.EntityManager);
        }

        private GameObject SpawnHealthBar(bool isVehicle, float3 position, float3 offset) =>
            Object.Instantiate(
                SelectHealthBarPrefab(isVehicle),
                position + offset,
                Quaternion.identity);

        private GameObject SelectHealthBarPrefab(bool isVehicle) =>
            isVehicle
            ? SystemAPI.ManagedAPI.GetSingleton<UIPrefabs>().VehicleHealthBar
            : SystemAPI.ManagedAPI.GetSingleton<UIPrefabs>().HealthBar;

        private void InitializeHealthBar(ref SystemState state, ref EntityCommandBuffer ecb, Entity entity, GameObject healthBar, bool isVehicle) {
            AddBasicHealthBarComponents(ref state, ref ecb, entity, healthBar);

            if (isVehicle)
                AddVehicleHealthBarComponents(ref state, ref ecb, entity, healthBar);
        }

        private void AddBasicHealthBarComponents(ref SystemState state, ref EntityCommandBuffer ecb, Entity entity, GameObject healthBar) {
            ecb.AddComponent(entity, new HealthBarUIReference { Value = healthBar });

            Slider slider = healthBar.GetComponentInChildren<Slider>();
            ecb.AddComponent(entity, new HealthBarSliderReference { Value = slider });

            HealthBarColor color = healthBar.GetComponent<HealthBarColor>();
            InitializeColor(ref state, entity, color);
            ecb.AddComponent(entity, new HealthBarColorReference { Value = color });

            ecb.RemoveComponent<HealthBarInitializationTag>(entity);
        }

        private void AddVehicleHealthBarComponents(ref SystemState state, ref EntityCommandBuffer ecb, Entity entity, GameObject healthBar) {
            HealthBarPlayerName playerName = healthBar.GetComponent<HealthBarPlayerName>();
            ecb.AddComponent(entity, new HealthBarPlayerNameReference { Value = playerName });
            ecb.AddComponent<InitializePlayerNameTag>(entity);

            HealthBarCounter counter = healthBar.GetComponent<HealthBarCounter>();
            ecb.AddComponent(entity, new HealthBarCounterReference { Value = counter });
        }

        private void InitializeColor(ref SystemState state, Entity entity, HealthBarColor color) {
            if (!SystemAPI.HasComponent<UnitTeam>(entity)) {
                color.ResetColor();
                return;
            }

            UnitTeam team = SystemAPI.GetComponent<UnitTeam>(entity);
            color.SetColorByTeam(team.Value);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(HealthBarClientSystemGroup))]
    [UpdateAfter(typeof(HealthBarInitializationSystem))]
    [UpdateBefore(typeof(HealthBarPositionUpdateSystem))]
    public partial struct HealthBarPlayerNameInitializationSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (playerName, uiPlayerName, entity)
                in SystemAPI.Query<PlayerName, HealthBarPlayerNameReference>()
                .WithAll<InitializePlayerNameTag>()
                .WithEntityAccess()) {

                uiPlayerName.Value.SetPlayerName(playerName.Value.ToString());

                ecb.RemoveComponent<InitializePlayerNameTag>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(HealthBarClientSystemGroup))]
    [UpdateAfter(typeof(HealthBarPlayerNameInitializationSystem))]
    [UpdateBefore(typeof(HealthBarValueUpdateSystem))]
    public partial struct HealthBarPositionUpdateSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            foreach (var (transform, healthOffset, healthBarUI)
                in SystemAPI.Query<LocalTransform, HealthBarOffset, HealthBarUIReference>()) {

                healthBarUI.Value.transform.position = transform.Position + healthOffset.Value;
            }
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(HealthBarClientSystemGroup))]
    [UpdateAfter(typeof(HealthBarPositionUpdateSystem))]
    [UpdateBefore(typeof(HealthBarCounterUpdateSystem))]
    public partial struct HealthBarValueUpdateSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            foreach (var (currentHealth, maxHealth, healthSlider)
                in SystemAPI.Query<CurrentHealthPoints, MaximalHealthPoints, HealthBarSliderReference>()) {

                healthSlider.Value.minValue = 0;
                healthSlider.Value.maxValue = maxHealth.Value;
                healthSlider.Value.value = currentHealth.Value;
            }
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(HealthBarClientSystemGroup))]
    [UpdateAfter(typeof(HealthBarValueUpdateSystem))]
    [UpdateBefore(typeof(HealthBarCleanupSystem))]
    public partial struct HealthBarCounterUpdateSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            foreach (var (currentHealth, uiCounter)
                in SystemAPI.Query<CurrentHealthPoints, HealthBarCounterReference>()) {

                uiCounter.Value.SetHealthCount(currentHealth.Value);
            }
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(HealthBarClientSystemGroup))]
    [UpdateAfter(typeof(HealthBarCounterUpdateSystem))]
    public partial struct HealthBarCleanupSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (healthBar, entity)
                in SystemAPI.Query<HealthBarUIReference>()
                .WithNone<LocalTransform>()
                .WithEntityAccess()) {

                Object.Destroy(healthBar.Value);
                ecb.RemoveComponent<HealthBarUIReference>(entity);
                ecb.RemoveComponent<HealthBarSliderReference>(entity);
                ecb.RemoveComponent<HealthBarColorReference>(entity);
                ecb.RemoveComponent<HealthBarCounterReference>(entity);
                ecb.RemoveComponent<HealthBarPlayerName>(entity);
            }
        }
    }
}
