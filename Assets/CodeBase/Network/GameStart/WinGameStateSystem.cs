using Unity.Entities;

namespace Assets.CodeBase.Network.GameStart
{
    [UpdateInGroup(typeof(NetworkProcessSystemGroup))]
    [UpdateAfter(typeof(ReportInGameStateSystem))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct WinGameStateSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGame>();
            state.RequireForUpdate<WinnerTeam>();
        }

        public void OnUpdate(ref SystemState state) {
            foreach (var winnerTeam
                in SystemAPI.Query<WinnerTeam>()) {

                UnityEngine.Debug.Log($"Game over. {winnerTeam.Value} won");
            }

            Entity gameStateEntity = SystemAPI.GetSingletonEntity<InGame>();
            state.EntityManager.RemoveComponent<InGame>(gameStateEntity);
        }
    }
}
