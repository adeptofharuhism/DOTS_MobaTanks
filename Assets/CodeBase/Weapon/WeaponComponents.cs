using Unity.Entities;

namespace Assets.CodeBase.Weapon
{
    public partial struct WeaponProjectilePrefab : IComponentData
    {
        public Entity Value;
    }

    public partial struct WeaponProjectileSpawnPoint : IComponentData
    {
        public Entity Value;
    }

    public partial struct WeaponReadyToFireTag : IComponentData { }

    public partial struct WeaponCooldown : IComponentData
    {
        public float Value;
    }

    public partial struct WeaponTimeOnCooldown : IComponentData
    {
        public float Value;
    }
}
