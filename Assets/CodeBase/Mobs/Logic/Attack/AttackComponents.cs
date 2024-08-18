using Unity.Entities;
using Unity.Mathematics;
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

    public enum AttackType
    {
        Melee,
        Projectile
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct MeleeAttackerTag : IComponentData { }
    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct ProjectileAttackerTag : IComponentData { }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct ProjectilePrefab : IComponentData
    {
        public Entity Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct ProjectileSpawnPoint : IComponentData
    {
        public Entity Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct ProjectileAimPosition : IComponentData
    {
        public float3 Value;
    }
}
