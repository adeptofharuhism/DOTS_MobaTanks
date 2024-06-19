using Unity.Entities;
using Unity.NetCode;

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
}