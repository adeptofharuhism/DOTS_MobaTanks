using Assets.CodeBase.Infrastructure.Services.WorldAccess;
using Assets.CodeBase.Player.PlayerCount;
using Unity.NetCode;

namespace Assets.CodeBase.Infrastructure.Services.WorldCommandSender
{
    public class WorldRpcSenderService : IWorldRpcSenderService
    {
        private readonly IWorldAccessService _worldAccess;

        public WorldRpcSenderService(IWorldAccessService worldAccessService) {
            _worldAccess = worldAccessService;
        }

        public void SendReadyRpc() =>
            _worldAccess.DefaultWorld.EntityManager
                .CreateEntity(typeof(ReadyRpc), typeof(SendRpcCommandRequest));
    }
}
