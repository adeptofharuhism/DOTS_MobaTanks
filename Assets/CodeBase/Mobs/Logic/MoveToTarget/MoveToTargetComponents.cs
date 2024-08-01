using Unity.Entities;
using Unity.Mathematics;

namespace Assets.CodeBase.Mobs.Logic.MoveToTarget
{
    public struct ChasedTarget : IComponentData
    {
        public Entity Value;
    }

    public struct ChasedTargetPosition : IComponentData {
        public float3 Value;
    }

    public struct SquaredChasedTargetDistance : IComponentData
    {
        public float Value;
    }
}
