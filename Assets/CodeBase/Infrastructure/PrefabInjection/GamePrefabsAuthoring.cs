using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Infrastructure.PrefabInjection
{
    public class GamePrefabsAuthoring : MonoBehaviour
    {
        [Header("Game Objects")]
        [SerializeField] private GameObject _vehicle;

        [Header("UI Objects")]
        [SerializeField] private GameObject _healthBar;
        [SerializeField] private GameObject _vehicleHealthBar;

        public GameObject Vehicle => _vehicle;
        public GameObject HealthBar => _healthBar;
        public GameObject VehicleHealthBar => _vehicleHealthBar;

        public class GamePrefabsBaker : Baker<GamePrefabsAuthoring>
        {
            public override void Bake(GamePrefabsAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new GamePrefabs {
                    Vehicle = GetEntity(authoring.Vehicle, TransformUsageFlags.Dynamic)
                });

                AddComponentObject(entity, new UIPrefabs {
                    HealthBar = authoring.HealthBar,
                    VehicleHealthBar = authoring.VehicleHealthBar,
                });
            }
        }
    }
}