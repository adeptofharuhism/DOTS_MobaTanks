using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace Assets.CodeBase.Mobs.Logic.MoveToPoint
{
    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct WaypointOffset : IComponentData
    {
        public ushort Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct WaypointAmount : IComponentData
    {
        public ushort Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct CurrentWaypointIndex : IComponentData
    {
        public ushort Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct CurrentWaypoint : IComponentData
    {
        public float3 Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct SquaredRequiredDistanceToWaypoint : IComponentData
    {
        public float Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct SquaredDistanceToWaypoint : IComponentData
    {
        public float Value;
    }

    public struct ShouldAdjustWaypointTag : IComponentData { }
}
