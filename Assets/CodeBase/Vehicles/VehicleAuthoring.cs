using Assets.CodeBase.Input;
using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Vehicles
{
    public class VehicleAuthoring : MonoBehaviour
    {
        public class VehicleBaker : Baker<VehicleAuthoring>
        {
            public override void Bake(VehicleAuthoring authoring) {
                Entity vehicle = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<VehicleTag>(vehicle);
                AddComponent<NewVehicleTag>(vehicle);
                AddComponent<NotOwnerVehicleTag>(vehicle);

                AddComponent<VehicleMovementInput>(vehicle);
            }
        }
    }
}