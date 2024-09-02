using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Mobs.Logic.TargetSearch
{
    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct TargetSearchCooldown : IComponentData
    {
        public float Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct TargetSearchCooldownTimeLeft : IComponentData
    {
        public float Value;
    }
}
