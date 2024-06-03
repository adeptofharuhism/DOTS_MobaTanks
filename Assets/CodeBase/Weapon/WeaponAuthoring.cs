using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Weapon
{
    public class WeaponAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject _projectileSpawnPoint;
        [SerializeField] private GameObject _projectilePrefab;

        public GameObject ProjectileSpawnPoint => _projectileSpawnPoint;
        public GameObject ProjectilePrefab => _projectilePrefab;

        public class WeaponBaker : Baker<WeaponAuthoring>
        {
            public override void Bake(WeaponAuthoring authoring) {
                Entity weapon = GetEntity(TransformUsageFlags.Dynamic);

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
