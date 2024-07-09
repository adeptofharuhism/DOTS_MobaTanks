using Assets.CodeBase.Infrastructure.PlayerCount;
using Unity.Entities;

namespace Assets.CodeBase.GameStates.PrepareForGame
{
    [UpdateInGroup(typeof(PrepareForGameStateSystemGroup), OrderLast = true)]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct EnterReportInGameStateSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<PrepareForGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            foreach (var (readyPlayers, connectedPlayers, minReadyPlayers, entity)
                in SystemAPI.Query<ReadyPlayersCount, ConnectedPlayerCount, MinReadyPlayersToStartGame>()
                .WithEntityAccess()) {

                if (minReadyPlayers.Value > readyPlayers.Value)
                    return;

                if (readyPlayers.Value != connectedPlayers.Value)
                    return;
            }

            Entity stateEntity = SystemAPI.GetSingletonEntity<PrepareForGameState>();

            state.EntityManager.RemoveComponent<PrepareForGameState>(stateEntity);
            state.EntityManager.AddComponent<ReportInGameState>(stateEntity);
        }
    }
}
