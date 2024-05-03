using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.CodeBase.Vehicles
{
    public class VehicleAuthoring : MonoBehaviour
    {
        [SerializeField] private int _wheelAmount;

        public int WheelAmount { get => _wheelAmount; set => _wheelAmount = value; }

        public class VehicleBaker : Baker<VehicleAuthoring>
        {
            public override void Bake(VehicleAuthoring authoring) {
                Entity vehicle = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<VehicleTag>(vehicle);
                AddComponent<NewVehicleTag>(vehicle);
                AddComponent<NotOwnerVehicleTag>(vehicle);

                AddComponent<VehicleMovementInput>(vehicle);
                AddComponent(vehicle, new VehicleWheelAmount { Value = authoring.WheelAmount });

                DynamicBuffer<VehicleSpringLengthCompressedBuffer> springBuffer = AddBuffer<VehicleSpringLengthCompressedBuffer>(vehicle);
                for (int i = 0; i < authoring.WheelAmount; i++)
                    springBuffer.Add(new VehicleSpringLengthCompressedBuffer {
                        Index = i,
                        Value = 0
                    });

                DynamicBuffer<VehicleRotationBuffer> rotationBuffer = AddBuffer<VehicleRotationBuffer>(vehicle);
                for (int i = 0; i < authoring.WheelAmount; i++)
                    rotationBuffer.Add(new VehicleRotationBuffer {
                        Index = i,
                        Value = quaternion.identity
                    });
            }
        }
    }
}