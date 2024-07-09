using Assets.CodeBase.Combat.Health;
using Assets.CodeBase.GameStates;
using Unity.Burst;
using Unity.Entities;

namespace Assets.CodeBase.Combat.Damage
{
    [UpdateInGroup(typeof(CombatSimulationSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
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
}
