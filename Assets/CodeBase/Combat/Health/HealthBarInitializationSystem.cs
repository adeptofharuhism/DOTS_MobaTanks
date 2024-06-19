using Assets.CodeBase.Infrastructure.PrefabInjection;
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
}
