using Assets.CodeBase.Network.GameStart;
using Unity.Burst;
using Unity.Entities;

namespace Assets.CodeBase.Infrastructure.Respawn
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct RespawnVehicleStartCooldownSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGame>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (spawnedEntity, timeToRespawn, cooldown, parametersEntity)
                in SystemAPI.Query<RespawnedEntity, RefRW<TimeToRespawn>, RespawnCooldown>()
                .WithAll<ChecksRespawnedEntityPresenceTag>()
                .WithEntityAccess()) {

                if (state.EntityManager.Exists(spawnedEntity.Value))
                    continue;

                timeToRespawn.ValueRW.Value = cooldown.Value;

                ecb.RemoveComponent<ChecksRespawnedEntityPresenceTag>(parametersEntity);
                ecb.AddComponent<OnRespawnCooldownTag>(parametersEntity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
