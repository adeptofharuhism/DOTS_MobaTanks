using Unity.Entities;

namespace Assets.CodeBase.Weapon.Projectile
{
    public partial struct ProjectileSpeed : IComponentData
    {
        public float Value;
    }

    public partial struct ProjectileDamage : IComponentData
    {
        public float Value;
    }
}
