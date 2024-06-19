using Assets.CodeBase.Combat.Health;
using Assets.CodeBase.Infrastructure.Destruction;
using Unity.Entities;

namespace Assets.CodeBase.Combat.Damage
{
    [UpdateInGroup(typeof(CombatSimulationSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct DamageCalculateSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            foreach (var (damageBuffer, frameDamage)
                in SystemAPI.Query<DynamicBuffer<DamageBufferElement>, RefRW<DamageThisFrame>>()) {

                foreach (DamageBufferElement damage in damageBuffer) {
                    frameDamage.ValueRW.Value += damage.Value;
                }

                damageBuffer.Clear();
            }
        }
    }

    [UpdateInGroup(typeof(CombatSimulationSystemGroup), OrderLast = true)]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct DamageApplySystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (frameDamage, health, entity)
                in SystemAPI.Query<RefRW<DamageThisFrame>, RefRW<CurrentHealthPoints>>()
                .WithEntityAccess()) {

                health.ValueRW.Value -= frameDamage.ValueRW.Value;
                frameDamage.ValueRW.Value = 0;

                if (health.ValueRW.Value <= 0)
                    ecb.AddComponent<DestroyEntityTag>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
