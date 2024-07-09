using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Infrastructure.PlayerCount
{
    public struct ReadyRpc : IRpcCommand { }
    public struct PlayerReady : IComponentData { }

    public struct ConnectedPlayerCount : IComponentData
    {
        public int Value;
    }

    public struct ReadyPlayersCount : IComponentData
    {
        public int Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct MinReadyPlayersToStartGame : IComponentData
    {
        public int Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct CountAsPlayerTag : IComponentData { }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct DecreaseConnectedPlayerCountOnCleanUpTag : ICleanupComponentData { }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct DecreaseReadyPlayerCountOnCleanUpTag : ICleanupComponentData { }
}
