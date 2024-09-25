using Assets.CodeBase.GameStates.PrepareForGame;
using Assets.CodeBase.Infrastructure.Services.ConnectionInfo;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using Zenject;

namespace Assets.CodeBase.Infrastructure.Services.WorldControl
{
    public class WorldControlService : IWorldControlService
    {
        private readonly IConnectionInfoService _connectionInfoService;

        [Inject]
        public WorldControlService(IConnectionInfoService connectionInfoService) {
            _connectionInfoService = connectionInfoService;
        }

        public void CreateServerWorld() {
            ClientServerBootstrap.CreateServerWorld(Constants.WorldNames.ServerWorld);
        }

        public void CreateClientWorld() {
            World.DefaultGameObjectInjectionWorld =
                ClientServerBootstrap.CreateClientWorld(Constants.WorldNames.ClientWorld);
        }

        public void StartWorlds() {
            bool isHost = 
                ClientServerBootstrap.ServerWorld != null;

            if (isHost) {
                StartServer();
                StartClient(_connectionInfoService.LocalIp);
            } else {
                StartClient(_connectionInfoService.ConnectionIp.Value);
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

        private void DisposeClientWorld() =>
            DisposeWorld(WorldFlags.GameClient);

        private void DisposeWorld(WorldFlags worldFlag) {
            foreach (World world in World.All)
                if (world.Flags == worldFlag) {
                    world.Dispose();
                    break;
                }
        }

        private void StartServer() {
            World serverWorld = ClientServerBootstrap.ServerWorld;

            NetworkEndpoint serverEndpoint = NetworkEndpoint.AnyIpv4.WithPort(_connectionInfoService.ConnectionPort.Value);

            using (EntityQuery networkDriverQuery =
                serverWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>()))
                networkDriverQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(serverEndpoint);
        }

        private void StartClient(string ipAddress) {
            World clientWorld = ClientServerBootstrap.ClientWorld;

            NetworkEndpoint connectionEndpoint = NetworkEndpoint.Parse(ipAddress, _connectionInfoService.ConnectionPort.Value);

            using (EntityQuery networkDriverQuery =
                clientWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>()))
                networkDriverQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(clientWorld.EntityManager, connectionEndpoint);

            Entity connectionDataEntity = clientWorld.EntityManager.CreateEntity();
            clientWorld.EntityManager.AddComponentData(connectionDataEntity, new ConnectionRequestData {
                PlayerName = _connectionInfoService.PlayerName.Value
            });
        }
    }
}
