using Unity.Entities;

namespace Assets.CodeBase.GameStates.ReportInGame
{
    [UpdateInGroup(typeof(ReportInGameStateSystemGroup), OrderLast = true)]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct EnterInGameStateSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<ReportInGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            Entity stateEntity = SystemAPI.GetSingletonEntity<ReportInGameState>();
            state.EntityManager.RemoveComponent<ReportInGameState>(stateEntity);
            state.EntityManager.AddComponent<InGameState>(stateEntity);
        }
    }
}
