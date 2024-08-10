using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace Assets.CodeBase.Mobs.Logic.MoveToTarget
{
    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct SearchForNewTargetTag : IComponentData, IEnableableComponent { }
    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct HasTargetInRangeTag : IComponentData, IEnableableComponent { }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct ChasedTarget : IComponentData
    {
        public Entity Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct ChasedTargetPosition : IComponentData
    {
        public float3 Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct SquaredChasedTargetDistance : IComponentData
    {
        public float Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct SquaredTargetSearchRange : IComponentData
    {
        public float Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct ChaseDuration : IComponentData
    {
        public float Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct ChaseTimeLeft : IComponentData
    {
        public float Value;
    }
}
