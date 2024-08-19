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
    public partial struct DecreaseConnectedPlayerCountSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<ConnectedPlayerCount>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            RefRW<ConnectedPlayerCount> playerCount = SystemAPI.GetSingletonRW<ConnectedPlayerCount>();

            foreach (var (tag, entity)
                in SystemAPI.Query<DecreaseConnectedPlayerCountOnCleanUpTag>()
                .WithNone<CountAsPlayerTag>()
                .WithEntityAccess()) {

                playerCount.ValueRW.Value--;

                ecb.RemoveComponent<DecreaseConnectedPlayerCountOnCleanUpTag>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
