using Unity.Entities;

namespace Assets.CodeBase.GameStates.InGame
{
    [UpdateInGroup(typeof(InGameStateSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct EnterReportEndGameStateSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
            state.RequireForUpdate<WinnerTeam>();
        }

        public void OnUpdate(ref SystemState state) {
            Entity stateEntity = SystemAPI.GetSingletonEntity<InGameState>();
            state.EntityManager.RemoveComponent<InGameState>(stateEntity);
            state.EntityManager.AddComponent<ReportEndGameState>(stateEntity);
        }
    }
}
