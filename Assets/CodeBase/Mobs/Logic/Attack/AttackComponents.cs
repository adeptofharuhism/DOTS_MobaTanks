using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Mobs.Logic.Attack
{
    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct SquaredAttackDistance : IComponentData
    {
        public float Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct AttackIsReadyTag : IComponentData, IEnableableComponent { }
    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct AttackIsOnCooldownTag : IComponentData, IEnableableComponent { }
    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct AttackHappenedThisFrameTag : IComponentData, IEnableableComponent { }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct AttackCooldown : IComponentData
    {
        public float Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct AttackCooldownTimeLeft : IComponentData
    {
        public float Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct AttackDamage : IComponentData
    {
        public float Value;
    }
}
