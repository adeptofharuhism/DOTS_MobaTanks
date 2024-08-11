using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace Assets.CodeBase.Effects.PostDeath
{
    [GhostComponent(PrefabType = GhostPrefabType.Client)]
    public struct PostDeathEffectInitializationData : IComponentData
    {
        public Entity EffectPfefab;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Client)]
    public struct PostDeathEffectPrefab : ICleanupComponentData
    {
        public Entity Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Client)]
    public struct PostDeathEffectPosition : ICleanupComponentData
    {
        public float3 Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Client)]
    public struct PostDeathEffectRotation : ICleanupComponentData
    {
        public quaternion Value;
    }
}
