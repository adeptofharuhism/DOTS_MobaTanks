using Assets.CodeBase.GameStates.InGame;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.GameStates.ReportEndGame
{
    [UpdateInGroup(typeof(ReportEndGameStateSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct SendRpcGameEndedSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<ReportEndGameState>();
            state.RequireForUpdate<WinnerTeam>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            Entity endGameRpc = ecb.CreateEntity();
            ecb.AddComponent(endGameRpc, new SendRpcCommandRequest());
            ecb.AddComponent(endGameRpc, new GoToEndGameStateRpc {
                Winner = SystemAPI.GetSingleton<WinnerTeam>().Value
            });

            ecb.Playback(state.EntityManager);
        }
    }
}
