using Assets.CodeBase.GameStates.PrepareForGame;
using Assets.CodeBase.Infrastructure.Services.ConnectionInfo;
using Assets.CodeBase.Infrastructure.Services.WorldAccess;
using Assets.CodeBase.Infrastructure.Services.WorldEvents;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using Zenject;

namespace Assets.CodeBase.Infrastructure.Services.WorldControl
{
    public class WorldControlService : IWorldControlService
    {
        private readonly IConnectionInfoService _connectionInfo;
        private readonly IWorldAccessService _worldAccess;
        private readonly IWorldEventSubscriptionControlService _worldEventSubscriptionControl;

        [Inject]
        public WorldControlService(
            IConnectionInfoService connectionInfoService,
            IWorldAccessService worldAccessService,
            IWorldEventSubscriptionControlService worldEventSubscriptionControlService) {

            _connectionInfo = connectionInfoService;
            _worldAccess = worldAccessService;
            _worldEventSubscriptionControl = worldEventSubscriptionControlService;
        }

        public void CreateServerWorld() =>
            ClientServerBootstrap.CreateServerWorld(Constants.WorldNames.ServerWorld);

        public void CreateClientWorld() {
            _worldAccess.DefaultWorld = 
                ClientServerBootstrap.CreateClientWorld(Constants.WorldNames.ClientWorld);

            _worldEventSubscriptionControl.SubscribeToWorldEvents();
        }

        public void StartWorlds() {
            bool isHost =
                ClientServerBootstrap.ServerWorld != null;
            
            if (isHost) {
                StartServer();
                StartClient(_connectionInfo.LocalIp);
            } else {
                StartClient(_connectionInfo.ConnectionIp.Value);
            }
        }

        public void DisposeNetworkWorlds() {
            DisposeServerWorld();
            DisposeClientWorld();
        }

        public void DisposeDefaultWorld() =>
            DisposeWorld(WorldFlags.Game);

        private void DisposeServerWorld() =>
            DisposeWorld(WorldFlags.GameServer);

        private void DisposeClientWorld() {
            _worldEventSubscriptionControl.UnsubscribeFromWorldEvents();

            DisposeWorld(WorldFlags.GameClient);
        }

        private void DisposeWorld(WorldFlags worldFlag) {
            foreach (World world in World.All)
                if (world.Flags == worldFlag) {
                    world.Dispose();
                    break;
                }
        }

        private void StartServer() {
            World serverWorld = ClientServerBootstrap.ServerWorld;

            NetworkEndpoint serverEndpoint = NetworkEndpoint.AnyIpv4.WithPort(_connectionInfo.ConnectionPort.Value);

            using (EntityQuery networkDriverQuery =
                serverWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>()))
                networkDriverQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(serverEndpoint);
        }

        private void StartClient(string ipAddress) {
            World clientWorld = ClientServerBootstrap.ClientWorld;

            NetworkEndpoint connectionEndpoint = NetworkEndpoint.Parse(ipAddress, _connectionInfo.ConnectionPort.Value);

            using (EntityQuery networkDriverQuery =
                clientWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>()))
                networkDriverQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(clientWorld.EntityManager, connectionEndpoint);

            Entity connectionDataEntity = clientWorld.EntityManager.CreateEntity();
            clientWorld.EntityManager.AddComponentData(connectionDataEntity, new ConnectionRequestData {
                PlayerName = _connectionInfo.PlayerName.Value
            });
        }
    }
}
