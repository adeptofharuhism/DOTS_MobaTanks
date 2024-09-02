using Assets.CodeBase.Teams;
using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Vehicles.Turrets
{
    [RequireComponent(typeof(TeamAuthoring))]
    [RequireComponent(typeof(VehicleAuthoring))]
    public class TurretAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject _turretSlot;
        [SerializeField] private TurretParameters _turretParameters;

        public GameObject TurretSlot => _turretSlot;
        public TurretParameters TurretParameters => _turretParameters;

        public class TurretBaker : Baker<TurretAuthoring>
        {
            public override void Bake(TurretAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<TurretUninitialized>(entity);

                AddComponent(entity, new TurretSlot {
                    Value = GetEntity(authoring.TurretSlot, TransformUsageFlags.Dynamic)
                });

                AddComponent<TurretModel>(entity);
                AddComponent(entity, new TurretModelPrefab {
                    Value = GetEntity(authoring.TurretParameters.TurretModelPrefab, TransformUsageFlags.Dynamic)
                });

                AddComponent<TurretWeapon>(entity);
                AddComponent(entity, new TurretWeaponPrefab {
                    Value = GetEntity(authoring.TurretParameters.TurretWeaponPrefab, TransformUsageFlags.Dynamic)
                });

                AddComponent(entity, new TurretRotationAngle { Value = 0 });
            }
        }
    }
}
