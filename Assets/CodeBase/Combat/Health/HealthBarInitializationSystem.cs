using Assets.CodeBase.Combat.Teams;
using Assets.CodeBase.Infrastructure.PrefabInjection;
using Assets.CodeBase.Vehicles;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.CodeBase.Combat.Health
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

                float3 spawnPosition = transform.Position + healthBarOffset.Value;
                GameObject healthBarPrefab =
                    (isVehicle)
                    ? SystemAPI.ManagedAPI.GetSingleton<UIPrefabs>().VehicleHealthBar
                    : SystemAPI.ManagedAPI.GetSingleton<UIPrefabs>().HealthBar;

                GameObject newHealthBar = Object.Instantiate(healthBarPrefab, spawnPosition, Quaternion.identity);
                Slider slider = newHealthBar.GetComponentInChildren<Slider>();

                HealthBarColor color = newHealthBar.GetComponent<HealthBarColor>();
                InitializeColor(ref state, entity, color);

                if (isVehicle) {
                    HealthBarPlayerName playerName = newHealthBar.GetComponent<HealthBarPlayerName>();
                    ecb.AddComponent(entity, new HealthBarPlayerNameReference { Value = playerName });
                    ecb.AddComponent<InitializePlayerNameTag>(entity);

                    HealthBarCounter counter = newHealthBar.GetComponent<HealthBarCounter>();
                    ecb.AddComponent(entity, new HealthBarCounterReference { Value = counter });
                }

                ecb.AddComponent(entity, new HealthBarUIReference { Value = newHealthBar });
                ecb.AddComponent(entity, new HealthBarSliderReference { Value = slider });
                ecb.AddComponent(entity, new HealthBarColorReference { Value = color });
                ecb.RemoveComponent<HealthBarInitializationTag>(entity);
            }

            ecb.Playback(state.EntityManager);
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
