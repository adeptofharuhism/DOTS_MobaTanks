using Assets.CodeBase.Combat.Health;
using Assets.CodeBase.Destruction;
using Assets.CodeBase.GameStates;
using Unity.Burst;
using Unity.Entities;

namespace Assets.CodeBase.Combat.Damage
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(DamageSystemGroup))]
    [UpdateBefore(typeof(DamageApplySystem))]
    public partial struct DamageCalculateSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
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

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(DamageSystemGroup))]
    [UpdateAfter(typeof(DamageCalculateSystem))]
    public partial struct DamageApplySystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
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
