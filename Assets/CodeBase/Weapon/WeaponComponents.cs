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
}
