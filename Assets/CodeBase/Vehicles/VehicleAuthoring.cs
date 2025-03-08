using Assets.CodeBase.Combat.Health;
using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Vehicles
{
    public class VehicleAuthoring : MonoBehaviour
    {
        [SerializeField] private int _wheelAmount;
        [SerializeField] private GameObject _itemSlot;

        public int WheelAmount { get => _wheelAmount; set => _wheelAmount = value; }
        public GameObject ItemSlot => _itemSlot;

        public class VehicleBaker : Baker<VehicleAuthoring>
        {
            public override void Bake(VehicleAuthoring authoring) {
                Entity vehicle = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<VehicleTag>(vehicle);
                AddComponent<NewVehicleTag>(vehicle);
                AddComponent<NotOwnerVehicleTag>(vehicle);

                AddComponent<VehicleMovementInput>(vehicle);
                AddComponent(vehicle, new VehicleWheelAmount { Value = authoring.WheelAmount });

                AddComponent(vehicle, new VehicleItemSlot {
                    Value = GetEntity(authoring.ItemSlot, TransformUsageFlags.Dynamic)
                });

                AddComponent(vehicle, new PlayerName { Value = string.Empty });
            }
        }
    }
}