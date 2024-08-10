using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Mobs.Logic
{
    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct EnterMoveToPointState : IComponentData, IEnableableComponent { }
    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct MoveToPointState : IComponentData, IEnableableComponent { }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct EnterMoveToTargetState : IComponentData, IEnableableComponent { }
    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct MoveToTargetState : IComponentData, IEnableableComponent { }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct EnterAttackState : IComponentData, IEnableableComponent { }
    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct AttackState : IComponentData, IEnableableComponent { }
}
