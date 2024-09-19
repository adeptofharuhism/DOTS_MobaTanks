using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Player.PlayerCount
{
    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct PlayerReady : IComponentData { }
    public struct ReadyRpc : IRpcCommand { }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct ConnectedPlayerCount : IComponentData
    {
        public int Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
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
