using Assets.CodeBase.Network.GameStart;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Weapon
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct WeaponCooldownSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGame>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (cooldown, timeOnCooldown, weapon)
                in SystemAPI.Query<WeaponCooldown, RefRW<WeaponTimeOnCooldown>>()
                .WithAll<WeaponOnCooldownTag>()
                .WithEntityAccess()) {

                timeOnCooldown.ValueRW.Value += SystemAPI.Time.DeltaTime;

                if (timeOnCooldown.ValueRW.Value > cooldown.Value) {
                    timeOnCooldown.ValueRW.Value = 0;

                    ecb.AddComponent<WeaponReadyToFireTag>(weapon);
                    ecb.RemoveComponent<WeaponOnCooldownTag>(weapon);
                }
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
