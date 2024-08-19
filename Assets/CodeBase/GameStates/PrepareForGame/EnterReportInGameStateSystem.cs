using Assets.CodeBase.Infrastructure.PlayerCount;
using Unity.Burst;
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

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (readyPlayers, connectedPlayers, minReadyPlayers)
                in SystemAPI.Query<ReadyPlayersCount, ConnectedPlayerCount, MinReadyPlayersToStartGame>()) {

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
