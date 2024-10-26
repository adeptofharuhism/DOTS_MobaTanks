using Assets.CodeBase.Infrastructure.Services.WorldAccess;
using Assets.CodeBase.Inventory;
using Assets.CodeBase.Player.PlayerCount;
using Unity.Entities;
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

        public void SendBuyItemRpc(int itemId) {
            Entity rpc = _worldAccess.DefaultWorld.EntityManager
                .CreateEntity(typeof(SendRpcCommandRequest));

            _worldAccess.DefaultWorld.EntityManager
                .AddComponentData(rpc, new BuyItemRpc { ItemId = itemId });
        }

        public void SendSellItemRpc(int slot) {
            Entity rpc = _worldAccess.DefaultWorld.EntityManager
                .CreateEntity(typeof(SendRpcCommandRequest));

            _worldAccess.DefaultWorld.EntityManager
                .AddComponentData(rpc, new SellItemRpc { Slot = slot });
        }
    }
}
