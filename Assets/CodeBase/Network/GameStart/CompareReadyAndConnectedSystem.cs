using Unity.Entities;

namespace Assets.CodeBase.Network.GameStart
{
    [UpdateInGroup(typeof(NetworkProcessSystemGroup))]
    [UpdateAfter(typeof(DecreaseConnectedPlayerCountSystem))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct CountingPlayersStateSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<CountingPlayersToStartGameTag>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (readyPlayers, connectedPlayers, minReadyPlayers, entity)
                in SystemAPI.Query<ReadyPlayersCount, ConnectedPlayerCount, MinReadyPlayersToStartGame>()
                .WithAll<CountingPlayersToStartGameTag>()
                .WithEntityAccess()) {

                if (minReadyPlayers.Value > readyPlayers.Value)
                    return;

                if (readyPlayers.Value != connectedPlayers.Value)
                    return;

                ecb.RemoveComponent<CountingPlayersToStartGameTag>(entity);

                ecb.AddComponent<ReportInGame>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
