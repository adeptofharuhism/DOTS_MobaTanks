using Assets.CodeBase.GameStates;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Infrastructure.PlayerCount
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(NetworkReceiveSystemGroup))]
    [UpdateBefore(typeof(GameStateSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct DecreaseReadyPlayerCountSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<ReadyPlayersCount>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            RefRW<ReadyPlayersCount> readyCount = SystemAPI.GetSingletonRW<ReadyPlayersCount>();

            foreach (var (tag, entity)
                in SystemAPI.Query<DecreaseReadyPlayerCountOnCleanUpTag>()
                .WithNone<PlayerReady>()
                .WithEntityAccess()) {

                readyCount.ValueRW.Value--;

                ecb.RemoveComponent<DecreaseReadyPlayerCountOnCleanUpTag>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
