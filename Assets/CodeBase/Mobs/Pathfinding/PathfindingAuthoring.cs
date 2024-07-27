using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.CodeBase.Mobs.Pathfinding
{
    public class PathfindingAuthoring : MonoBehaviour
    {
        public class PathfindingBaker : Baker<PathfindingAuthoring>
        {
            public override void Bake(PathfindingAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new PathDestination { Value = float3.zero });
            }
        }
    }
}
