using Unity.Entities;

namespace Assets.CodeBase.Mobs.Logic
{
    public struct EnterMoveToPointState : IComponentData, IEnableableComponent { }
    public struct MoveToPointState : IComponentData, IEnableableComponent { }

    public struct EnterMoveToTargetState : IComponentData, IEnableableComponent { }
    public struct MoveToTargetState : IComponentData, IEnableableComponent { }

    public struct EnterAttackState : IComponentData, IEnableableComponent { }
    public struct AttackState : IComponentData, IEnableableComponent { }
}
