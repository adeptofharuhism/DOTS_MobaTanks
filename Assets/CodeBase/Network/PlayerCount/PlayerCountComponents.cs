using Unity.Entities;

namespace Assets.CodeBase.Network.PlayerCount
{
    public struct ConnectedPlayerCount : IComponentData
    {
        public int Value;
    }

    public struct ReadyPlayersCount : IComponentData
    {
        public int Value;
    }

    public struct MinReadyPlayersToStartGame : IComponentData
    {
        public int Value;
    }

    public struct CountAsPlayerTag : IComponentData { }
    public struct DecreaseConnectedPlayerCountOnCleanUpTag : IComponentData { }
}
