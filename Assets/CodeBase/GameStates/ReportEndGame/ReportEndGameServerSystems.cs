using Assets.CodeBase.GameStates.InGame;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.GameStates.ReportEndGame
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(ReportEndGameStateSystemGroup))]
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

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(ReportEndGameStateSystemGroup), OrderLast = true)]
    public partial struct EnterEndGameStateSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<ReportEndGameState>();
            state.RequireForUpdate<WinnerTeam>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            Entity stateEntity = SystemAPI.GetSingletonEntity<ReportEndGameState>();
            state.EntityManager.RemoveComponent<ReportEndGameState>(stateEntity);
            state.EntityManager.AddComponent<EndGameState>(stateEntity);
        }
    }
}
