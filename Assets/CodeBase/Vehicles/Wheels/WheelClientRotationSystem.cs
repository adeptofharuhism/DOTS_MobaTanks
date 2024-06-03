using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Assets.CodeBase.Vehicles.Wheels
{
    [UpdateAfter(typeof(TransformSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct WheelClientRotationSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            foreach (var (rotationParameters, forceCastPoint, parent)
                in SystemAPI.Query<WheelRotationParameters, WheelForceCastPoint, WheelParent>()) {

                RefRO<VehicleMovementInput> movementInput = SystemAPI.GetComponentRO<VehicleMovementInput>(parent.Value);

                RefRW<LocalTransform> forceCastTransform = SystemAPI.GetComponentRW<LocalTransform>(forceCastPoint.Value);

                int clockwiseMultiplier = rotationParameters.RotatesClockwise ? 1 : -1;
                float rotationAngle = movementInput.ValueRO.Value.x * clockwiseMultiplier * rotationParameters.MaxRotationAngle;

                quaternion currentRotation = quaternion.Euler(0, math.radians(rotationAngle), 0);

                forceCastTransform.ValueRW.Rotation = currentRotation;
            }
        }
    }
}
