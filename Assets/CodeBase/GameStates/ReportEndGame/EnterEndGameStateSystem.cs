using Assets.CodeBase.GameStates.InGame;
using Unity.Entities;

namespace Assets.CodeBase.GameStates.ReportEndGame
{
    [UpdateInGroup(typeof(ReportEndGameStateSystemGroup), OrderLast = true)]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct EnterEndGameStateSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<ReportEndGameState>();
            state.RequireForUpdate<WinnerTeam>();
        }

        public void OnUpdate(ref SystemState state) {
            Entity stateEntity = SystemAPI.GetSingletonEntity<ReportEndGameState>();
            state.EntityManager.RemoveComponent<ReportEndGameState>(stateEntity);
            state.EntityManager.AddComponent<EndGameState>(stateEntity);
        }
    }
}
