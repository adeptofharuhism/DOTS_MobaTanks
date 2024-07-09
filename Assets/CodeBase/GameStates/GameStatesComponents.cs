using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.GameStates
{
    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct PrepareForGameState : IComponentData { }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct ReportInGameState : IComponentData { }

    public struct InGameState : IComponentData { }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct ReportEndGameState : IComponentData { }

    public struct EndGameState : IComponentData { }
}
