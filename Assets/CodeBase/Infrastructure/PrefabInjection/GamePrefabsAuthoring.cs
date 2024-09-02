using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Infrastructure.PrefabInjection
{
    public class GamePrefabsAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject _playerEntity;

        [Header("Game Objects")]
        [SerializeField] private GameObject _vehicle;
        [SerializeField] private GameObject _base;

        [Header("UI Objects")]
        [SerializeField] private GameObject _healthBar;
        [SerializeField] private GameObject _vehicleHealthBar;

        public GameObject PlayerEntity => _playerEntity;

        public GameObject Vehicle => _vehicle;
        public GameObject Base => _base;

        public GameObject HealthBar => _healthBar;
        public GameObject VehicleHealthBar => _vehicleHealthBar;

        public class GamePrefabsBaker : Baker<GamePrefabsAuthoring>
        {
            public override void Bake(GamePrefabsAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new GamePrefabs {
                    Player = GetEntity(authoring.PlayerEntity, TransformUsageFlags.None),
                    Vehicle = GetEntity(authoring.Vehicle, TransformUsageFlags.Dynamic),
                    Base = GetEntity(authoring.Base, TransformUsageFlags.Dynamic)
                });

                AddComponentObject(entity, new UIPrefabs {
                    HealthBar = authoring.HealthBar,
                    VehicleHealthBar = authoring.VehicleHealthBar,
                });
            }
        }
    }
}