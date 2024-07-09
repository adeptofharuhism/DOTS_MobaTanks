using Assets.CodeBase.GameStates;
using Unity.Burst;
using Unity.Entities;

namespace Assets.CodeBase.Infrastructure.Respawn
{
    [UpdateAfter(typeof(RespawnVehicleStartCooldownSystem))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct RespawnVehicleCooldownSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (timeToRespawn, entity)
                in SystemAPI.Query<RefRW<TimeToRespawn>>()
                .WithAll<OnRespawnCooldownTag>()
                .WithEntityAccess()) {

                timeToRespawn.ValueRW.Value -= SystemAPI.Time.DeltaTime;

                if (timeToRespawn.ValueRW.Value > 0)
                    continue;

                ecb.RemoveComponent<OnRespawnCooldownTag>(entity);
                ecb.AddComponent<ShouldRespawnTag>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
