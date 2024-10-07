using Assets.CodeBase.Player.PlayerCount;
using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Infrastructure.Services.WorldCommandSender
{
    public class WorldRpcSenderService : IWorldRpcSenderService
    {
        public void SendReadyRpc() =>
            World.DefaultGameObjectInjectionWorld
                .EntityManager.CreateEntity(typeof(ReadyRpc), typeof(SendRpcCommandRequest));
    }
}
