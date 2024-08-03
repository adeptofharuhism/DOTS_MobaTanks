using Unity.Entities;

namespace Assets.CodeBase.Mobs.Logic
{
    public struct EnterMoveToPointState: IComponentData { }
    public struct MoveToPointState : IComponentData { }

    public struct EnterMoveToTargetState : IComponentData { }
    public struct MoveToTargetState : IComponentData { }

    public struct EnterAttackState : IComponentData { }
    public struct AttackState : IComponentData { }
}
