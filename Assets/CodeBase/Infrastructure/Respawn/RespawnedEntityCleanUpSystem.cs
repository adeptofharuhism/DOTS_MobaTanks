using Assets.CodeBase.Infrastructure.Destruction;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Infrastructure.Respawn
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    [UpdateBefore(typeof(DestroyEntitySystem))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct RespawnedEntityCleanUpSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (spawnedEntity, parametersEntity)
                in SystemAPI.Query<RespawnedEntity>()
                .WithNone<VehicleRespawnParameters>()
                .WithEntityAccess()) {

                if (state.EntityManager.Exists(spawnedEntity.Value))
                    ecb.AddComponent<DestroyEntityTag>(spawnedEntity.Value);

                ecb.RemoveComponent<RespawnedEntity>(parametersEntity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
