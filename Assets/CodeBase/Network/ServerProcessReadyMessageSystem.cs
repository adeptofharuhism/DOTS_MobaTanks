﻿using Assets.CodeBase.Network.PlayerCount;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Network
{
    [UpdateInGroup(typeof(NetworkProcessSystemGroup), OrderFirst = true)]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct ServerProcessReadyMessageSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            EntityQueryBuilder readyRequestQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<ReadyRpc, ReceiveRpcCommandRequest>();
            state.RequireForUpdate(state.GetEntityQuery(readyRequestQuery));

            state.RequireForUpdate<ReadyPlayersCount>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            RefRW<ReadyPlayersCount> readyPlayers = SystemAPI.GetSingletonRW<ReadyPlayersCount>();

            foreach (var (commandSource, commandEntity)
                in SystemAPI.Query<ReceiveRpcCommandRequest>()
                .WithAll<ReadyRpc>()
                .WithEntityAccess()) {

                ecb.DestroyEntity(commandEntity);

                ecb.AddComponent<PlayerReady>(commandSource.SourceConnection);

                readyPlayers.ValueRW.Value++;
            }

            ecb.Playback(state.EntityManager);
        }
    }
}