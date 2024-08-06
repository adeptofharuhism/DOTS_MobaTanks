using Assets.CodeBase.GameStates;
using Assets.CodeBase.Targeting;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Weapon
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderFirst = true)]
    public partial struct WeaponCooldownSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (cooldown, timeOnCooldown, weapon)
                in SystemAPI.Query<WeaponCooldown, RefRW<WeaponTimeOnCooldown>>()
                .WithEntityAccess()) {

                timeOnCooldown.ValueRW.Value -= SystemAPI.Time.DeltaTime;

                if (timeOnCooldown.ValueRW.Value > 0)
                    continue;

                timeOnCooldown.ValueRW.Value = cooldown.Value;

                ecb.SetComponentEnabled<Targeter>(weapon, true);
                ecb.SetComponentEnabled<WeaponReadyToFireTag>(weapon, true);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
