using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Weapon.Projectile
{
    public class ProjectileAuthoring : MonoBehaviour
    {
        [SerializeField] private float _projectileSpeed;
        [SerializeField] private float _projectileDamage;

        public float ProjectileSpeed => _projectileSpeed;
        public float ProjectileDamage => _projectileDamage;

        public class ProjectileBaker : Baker<ProjectileAuthoring>
        {
            public override void Bake(ProjectileAuthoring authoring) {
                Entity projectile = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(projectile, new ProjectileSpeed { Value = authoring.ProjectileSpeed });
                AddComponent(projectile, new ProjectileDamage { Value = authoring.ProjectileDamage });
            }
        }
    }
}