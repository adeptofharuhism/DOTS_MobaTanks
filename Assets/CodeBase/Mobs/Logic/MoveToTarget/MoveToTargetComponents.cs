using Unity.Entities;
using Unity.Mathematics;

namespace Assets.CodeBase.Mobs.Logic.MoveToTarget
{
    public struct SearchForNewTargetTag : IComponentData { }
    public struct HasTargetInRangeTag : IComponentData { }

    public struct ChasedTarget : IComponentData
    {
        public Entity Value;
    }

    public struct ChasedTargetPosition : IComponentData
    {
        public float3 Value;
    }

    public struct SquaredChasedTargetDistance : IComponentData
    {
        public float Value;
    }

    public struct SquaredTargetSearchRange : IComponentData
    {
        public float Value;
    }

    public struct ChaseDuration : IComponentData
    {
        public float Value;
    }

    public struct ChaseTimeLeft : IComponentData
    {
        public float Value;
    }
}
