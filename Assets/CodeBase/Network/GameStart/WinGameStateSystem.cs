using Unity.Entities;

namespace Assets.CodeBase.Network.GameStart
{
    [UpdateInGroup(typeof(NetworkProcessSystemGroup))]
    [UpdateAfter(typeof(ReportInGameStateSystem))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct InGameStateSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGame>();
            state.RequireForUpdate<WinnerTeam>();
        }

        public void OnUpdate(ref SystemState state) {
            Entity stateEntity = SystemAPI.GetSingletonEntity<InGame>();
            state.EntityManager.RemoveComponent<InGame>(stateEntity);
            state.EntityManager.AddComponent<ReportEndGame>(stateEntity);
        }
    }
}
