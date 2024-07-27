using Unity.Entities;
using Unity.Mathematics;

namespace Assets.CodeBase.Mobs.Pathfinding
{
    public struct PathDestination : IComponentData
    {
        public float3 Value;
    }
}
