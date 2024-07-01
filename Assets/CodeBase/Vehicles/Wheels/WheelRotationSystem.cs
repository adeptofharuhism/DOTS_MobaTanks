using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace Assets.CodeBase.Vehicles.Wheels
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    [UpdateAfter(typeof(WheelSetRotationInputSystem))]
    public partial struct WheelRotationSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (rotationInput, rotationParameters, forceCastPoint)
                in SystemAPI.Query<WheelRotationInput, WheelRotationParameters, WheelForceCastPoint>()
                .WithAll<WheelHasRotationTag, WheelInitializedTag, Simulate>()) {

                RefRW<LocalTransform> forceCastTransform = SystemAPI.GetComponentRW<LocalTransform>(forceCastPoint.Value);

                forceCastTransform.ValueRW.Rotation = 
                    CalculateRotationQuaternion(
                        rotationInput.Value, 
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