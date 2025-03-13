using Unity.Entities;
using Unity.Mathematics;

namespace Assets.CodeBase.Effects.Following
{
    public struct FollowTarget : IComponentData
    {
        public Entity Value;
    }

    public struct FollowOffset : IComponentData
    {
        public float3 Value;
    }
}
