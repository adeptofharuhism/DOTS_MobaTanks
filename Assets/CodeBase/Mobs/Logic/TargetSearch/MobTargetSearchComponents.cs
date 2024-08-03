using Unity.Entities;

namespace Assets.CodeBase.Mobs.Logic.TargetSearch
{
    public struct TargetSearchCooldown : IComponentData
    {
        public float Value;
    }

    public struct TargetSearchCooldownTimeLeft : IComponentData
    {
        public float Value;
    }

    public struct MobReadyToSearchTargetTag : IComponentData { }
}
