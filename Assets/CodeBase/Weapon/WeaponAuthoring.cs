using Assets.CodeBase.Teams;
using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Weapon
{
    [RequireComponent(typeof(TeamAuthoring))]
    [RequireComponent(typeof(Targeting.TargeterAuthoring))]
    public class WeaponAuthoring : MonoBehaviour
    {
        [Header("Projectile spawn parameters")]
        [SerializeField] private GameObject _projectileSpawnPoint;
        [SerializeField] private GameObject _projectilePrefab;
        [Header("Weapon parameters")]
        [SerializeField] private float _shotCooldown = 1f;

        public GameObject ProjectileSpawnPoint => _projectileSpawnPoint;
        public GameObject ProjectilePrefab => _projectilePrefab;

        public float ShotCooldown => _shotCooldown;

        public class WeaponBaker : Baker<WeaponAuthoring>
        {
            public override void Bake(WeaponAuthoring authoring) {
                Entity weapon = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<WeaponReadyToFireTag>(weapon);
                SetComponentEnabled<WeaponReadyToFireTag>(weapon, false);

                AddComponent(weapon, new WeaponCooldown { Value = authoring.ShotCooldown });
                AddComponent(weapon, new WeaponTimeOnCooldown { Value = authoring.ShotCooldown });

                AddComponent(weapon, new WeaponProjectilePrefab {
                    Value = GetEntity(authoring.ProjectilePrefab, TransformUsageFlags.Dynamic)
                });
                AddComponent(weapon, new WeaponProjectileSpawnPoint {
                    Value = GetEntity(authoring.ProjectileSpawnPoint, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}
