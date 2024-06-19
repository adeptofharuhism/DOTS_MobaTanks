using Assets.CodeBase.Infrastructure.PrefabInjection;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.CodeBase.Combat.Health
{
    [UpdateInGroup(typeof(TransformSystemGroup), OrderFirst = true)]
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

                float3 spawnPosition = transform.Position + healthBarOffset.Value;
                GameObject healthBarPrefab = SystemAPI.ManagedAPI.GetSingleton<UIPrefabs>().HealthBar;

                GameObject newHealthBar = Object.Instantiate(healthBarPrefab, spawnPosition, Quaternion.identity);
                Slider slider = newHealthBar.GetComponentInChildren<Slider>();

                ecb.AddComponent(entity, new HealthBarUIReference { Value = newHealthBar });
                ecb.AddComponent(entity, new HealthBarSliderReference { Value = slider });
                ecb.RemoveComponent<HealthBarInitializationTag>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [UpdateInGroup(typeof(TransformSystemGroup))]
    [UpdateAfter(typeof(HealthBarInitializationSystem))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct HealthBarPositionUpdateSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            foreach (var (transform, healthOffset, healthBarUI)
                in SystemAPI.Query<LocalTransform, HealthBarOffset, HealthBarUIReference>()) {

                healthBarUI.Value.transform.position = transform.Position + healthOffset.Value;
            }
        }
    }

    [UpdateInGroup(typeof(TransformSystemGroup))]
    [UpdateAfter(typeof(HealthBarInitializationSystem))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
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

    [UpdateInGroup(typeof(TransformSystemGroup), OrderLast = true)]
    public partial struct HealthBarCleanupSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = 
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (healthBar, slider, entity)
                in SystemAPI.Query<HealthBarUIReference, HealthBarSliderReference>()
                .WithNone<LocalTransform>()
                .WithEntityAccess()) {

                Object.Destroy(healthBar.Value);
                ecb.RemoveComponent<HealthBarUIReference>(entity);
                ecb.RemoveComponent<HealthBarSliderReference>(entity);
            }
        }
    }
}
