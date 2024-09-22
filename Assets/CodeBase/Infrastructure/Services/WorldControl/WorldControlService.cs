using Assets.CodeBase.GameStates.PrepareForGame;
using Assets.CodeBase.Infrastructure.Services.ConnectionInfo;
using System.Transactions;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using Zenject;

namespace Assets.CodeBase.Infrastructure.Services.WorldControl
{
    public class WorldControlService : IWorldControlService
    {
        private readonly IConnectionInfoService _connectionInfoService;

        private bool _isHost = false;

        [Inject]
        public WorldControlService(IConnectionInfoService connectionInfoService) {
            _connectionInfoService = connectionInfoService;
        }

        public void SetHost(bool isHost) =>
            _isHost = isHost;

        public void CreateWorlds() {
            DisposeWorlds();

            if (_isHost) {
                StartServer();
                StartClient(_connectionInfoService.LocalHost);
            } else
                StartClient(_connectionInfoService.ConnectionIP);
        }

        public void DisposeWorlds() {
            foreach (World world in World.All)
                if (world.Flags == WorldFlags.Game) {
                    world.Dispose();
                    break;
                }
        }

        private void StartServer() {
            World serverWorld = ClientServerBootstrap.CreateServerWorld(Constants.WorldNames.ServerWorld);

            NetworkEndpoint serverEndpoint = NetworkEndpoint.AnyIpv4.WithPort(_connectionInfoService.ConnectionPort);

            using (EntityQuery networkDriverQuery =
                serverWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>()))
                networkDriverQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(serverEndpoint);
        }

        private void StartClient(string ipAddress) {
            World clientWorld = ClientServerBootstrap.CreateClientWorld(Constants.WorldNames.ClientWorld);
            World.DefaultGameObjectInjectionWorld = clientWorld;

            NetworkEndpoint connectionEndpoint = NetworkEndpoint.Parse(ipAddress, _connectionInfoService.ConnectionPort);

            using (EntityQuery networkDriverQuery =
                clientWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>()))
                networkDriverQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(clientWorld.EntityManager, connectionEndpoint);

            Entity connectionDataEntity = clientWorld.EntityManager.CreateEntity();
            clientWorld.EntityManager.AddComponentData(connectionDataEntity, new ConnectionRequestData {
                PlayerName = _connectionInfoService.PlayerName
            });
        }
    }
}
