using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Weapon
{
    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public partial struct WeaponProjectilePrefab : IComponentData
    {
        public Entity Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public partial struct WeaponProjectileSpawnPoint : IComponentData
    {
        public Entity Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public partial struct WeaponReadyToFireTag : IComponentData, IEnableableComponent { }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public partial struct WeaponCooldown : IComponentData
    {
        public float Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public partial struct WeaponTimeOnCooldown : IComponentData
    {
        public float Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct ShouldInitializeWeaponGroup : IComponentData { }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct WeaponGroupSlot : IComponentData
    {
        public Entity Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct WeaponBufferElement : IBufferElementData
    {
        public Entity WeaponPrefab;
    }
}
