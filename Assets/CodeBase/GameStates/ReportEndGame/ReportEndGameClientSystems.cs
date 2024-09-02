using Assets.CodeBase.GameStates.ReportEndGame;
using Assets.CodeBase.Teams;
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.GameStates.GameStart
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(ReportEndGameStateSystemGroup))]
    public partial class ClientEnterEndGameSystem : SystemBase
    {
        public Action<TeamType> OnEndGame;

        protected override void OnCreate() {
            EntityQueryBuilder endGameCommandQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<GoToEndGameStateRpc, ReceiveRpcCommandRequest>();

            RequireForUpdate(GetEntityQuery(endGameCommandQuery));
            RequireForUpdate<InGameState>();
        }

        protected override void OnUpdate() {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            ecb.DestroyEntity(SystemAPI.GetSingletonEntity<InGameState>());

            foreach (var (endGameData, commandSource, commandEntity)
                in SystemAPI.Query<GoToEndGameStateRpc, ReceiveRpcCommandRequest>()
                .WithEntityAccess()) {

                ecb.DestroyEntity(commandEntity);

                OnEndGame?.Invoke(endGameData.Winner);
            }

            ecb.Playback(EntityManager);
        }
    }
}
