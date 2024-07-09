using Assets.CodeBase.Combat.Teams;
using Assets.CodeBase.GameStates.ReportEndGame;
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.GameStates.GameStart
{
    [UpdateInGroup(typeof(ReportEndGameStateSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
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
