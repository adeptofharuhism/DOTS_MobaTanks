using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Weapon.Projectile
{
    public class ProjectileAuthoring : MonoBehaviour
    {
        [SerializeField] private float _projectileSpeed;

        public float ProjectileSpeed => _projectileSpeed;

        public class ProjectileBaker : Baker<ProjectileAuthoring>
        {
            public override void Bake(ProjectileAuthoring authoring) {
                Entity projectile = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(projectile, new ProjectileSpeed { Value = authoring.ProjectileSpeed });
            }
        }
    }
}