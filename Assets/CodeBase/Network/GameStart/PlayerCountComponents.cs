using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Network.GameStart
{
    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct CountingPlayersToStartGameTag : IComponentData { }
    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct ReportInGame : IComponentData { }
    public struct InGame : IComponentData { }

    public struct GoInGameStateRpc : IRpcCommand { }

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
