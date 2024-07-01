using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Assets.CodeBase.Vehicles.Wheels
{
    [UpdateAfter(typeof(TransformSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct WheelClientRotationSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (rotationParameters, forceCastPoint, parent)
                in SystemAPI.Query<WheelRotationParameters, WheelForceCastPoint, WheelParent>()) {

                RefRO<VehicleMovementInput> movementInput = SystemAPI.GetComponentRO<VehicleMovementInput>(parent.Value);

                RefRW<LocalTransform> forceCastTransform = SystemAPI.GetComponentRW<LocalTransform>(forceCastPoint.Value);

                forceCastTransform.ValueRW.Rotation =
                    CalculateRotationQuaternion(
                        movementInput.ValueRO.Value.x,
                        rotationParameters.MaxRotationAngle,
                        rotationParameters.RotatesClockwise);
            }
        }

        [BurstCompile]
        private quaternion CalculateRotationQuaternion(float rotationInput, float maxRotationAngle, bool rotatesClockwise) =>
            quaternion.Euler(
                0,
                math.radians(CalculateRotationAngle(rotationInput, maxRotationAngle, rotatesClockwise)),
                0);

        [BurstCompile]
        private float CalculateRotationAngle(float rotationInput, float maxRotationAngle, bool rotatesClockwise) =>
            rotationInput * (rotatesClockwise ? 1 : -1) * maxRotationAngle;
    }
}
