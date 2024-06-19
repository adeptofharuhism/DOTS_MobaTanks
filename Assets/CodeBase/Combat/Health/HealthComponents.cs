using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.CodeBase.Combat.Health
{
    public partial struct MaximalHealthPoints : IComponentData
    {
        [GhostField(Quantization = 0)] public float Value;
    }

    public partial struct CurrentHealthPoints : IComponentData
    {
        [GhostField(Quantization = 0)] public float Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct DamageBufferElement : IBufferElementData
    {
        public float Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct DamageThisFrame : IComponentData
    {
        public float Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Client)]
    public struct HealthBarInitializationTag : IComponentData { }

    [GhostComponent(PrefabType = GhostPrefabType.Client)]
    public struct HealthBarOffset : IComponentData
    {
        public float3 Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Client)]
    public class HealthBarUIReference : ICleanupComponentData
    {
        public GameObject Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Client)]
    public class HealthBarSliderReference : ICleanupComponentData
    {
        public Slider Value;
    }
}