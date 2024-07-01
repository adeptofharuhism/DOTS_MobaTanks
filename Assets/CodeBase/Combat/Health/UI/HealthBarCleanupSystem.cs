using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Assets.CodeBase.Combat.Health.UI
{
    [UpdateInGroup(typeof(HealthBarClientSystemGroup), OrderLast = true)]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
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
