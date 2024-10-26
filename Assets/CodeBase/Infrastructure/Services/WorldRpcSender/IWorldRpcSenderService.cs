namespace Assets.CodeBase.Infrastructure.Services.WorldCommandSender
{
    public interface IWorldRpcSenderService
    {
        void SendBuyItemRpc(int itemId);
        void SendReadyRpc();
        void SendSellItemRpc(int slot);
    }
}
