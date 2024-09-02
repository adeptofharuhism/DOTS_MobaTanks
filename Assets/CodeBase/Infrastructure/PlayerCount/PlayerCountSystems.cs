using Assets.CodeBase.GameStates;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Infrastructure.PlayerCount
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(PlayerCountSystemGroup), OrderFirst = true)]
    public partial struct ServerProcessReadyMessageSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            EntityQueryBuilder readyRequestQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<ReadyRpc, ReceiveRpcCommandRequest>();
            state.RequireForUpdate(state.GetEntityQuery(readyRequestQuery));

            state.RequireForUpdate<ReadyPlayersCount>();
            state.RequireForUpdate<PrepareForGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            RefRW<ReadyPlayersCount> readyPlayers = SystemAPI.GetSingletonRW<ReadyPlayersCount>();

            foreach (var (commandSource, commandEntity)
                in SystemAPI.Query<ReceiveRpcCommandRequest>()
                .WithAll<ReadyRpc>()
                .WithEntityAccess()) {

                ecb.DestroyEntity(commandEntity);

                ecb.AddComponent<PlayerReady>(commandSource.SourceConnection);
                ecb.AddComponent<DecreaseReadyPlayerCountOnCleanUpTag>(commandSource.SourceConnection);

                readyPlayers.ValueRW.Value++;
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(PlayerCountSystemGroup))]
    public partial struct DecreaseConnectedPlayerCountSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<ConnectedPlayerCount>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            RefRW<ConnectedPlayerCount> playerCount = SystemAPI.GetSingletonRW<ConnectedPlayerCount>();

            foreach (Entity entity
                in SystemAPI.QueryBuilder()
                .WithAll<DecreaseConnectedPlayerCountOnCleanUpTag>()
                .WithNone<CountAsPlayerTag>()
                .Build().ToEntityArray(Allocator.Temp)) {

                playerCount.ValueRW.Value--;

                ecb.RemoveComponent<DecreaseConnectedPlayerCountOnCleanUpTag>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(PlayerCountSystemGroup))]
    public partial struct DecreaseReadyPlayerCountSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<ReadyPlayersCount>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            RefRW<ReadyPlayersCount> readyCount = SystemAPI.GetSingletonRW<ReadyPlayersCount>();

            foreach (Entity entity
                in SystemAPI.QueryBuilder()
                .WithAll<DecreaseReadyPlayerCountOnCleanUpTag>()
                .WithNone<PlayerReady>()
                .Build().ToEntityArray(Allocator.Temp)) {

                readyCount.ValueRW.Value--;

                ecb.RemoveComponent<DecreaseReadyPlayerCountOnCleanUpTag>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
