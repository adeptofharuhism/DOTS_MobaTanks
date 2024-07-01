using Assets.CodeBase.Combat.Teams;
using Assets.CodeBase.Infrastructure.PrefabInjection;
using Assets.CodeBase.Vehicles;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.CodeBase.Combat.Health.UI
{
    [UpdateInGroup(typeof(HealthBarClientSystemGroup), OrderFirst = true)]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
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
}
