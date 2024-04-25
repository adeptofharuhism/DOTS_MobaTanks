using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace Assets.CodeBase.Vehicles.Wheels
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(WheelAccelerationEnableSystem))]
    public partial struct WheelAccelerationForceCalculationSystem : ISystem
    {
        private const float Epsilon = 1e-06f;
        private const float MaxSpeed = 14f;
        private const float MaxSpeedBackwards = -6f;
        private const float HardBreakingEngineCoefficient = 0;
        private const float HardBreakingForceMultiplier = 1.42f;

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (accelerationInput, axisProjectedVelocity, forceCastPosition, wheel)
                in SystemAPI.Query<WheelAccelerationInput, WheelAxisProjectedVelocity, WheelForceCastPoint>()
                .WithAll<WheelInitializedTag, WheelHasGroundContactTag, WheelAcceleratingTag>()
                .WithEntityAccess()) {

                RefRO<LocalToWorld> forceCastTransform = SystemAPI.GetComponentRO<LocalToWorld>(forceCastPosition.Value);

                float3 forceCastForward = forceCastTransform.ValueRO.Forward;

                float zForceValue;
                if (accelerationInput.Value > Epsilon) {
                    if (axisProjectedVelocity.Value.z > MaxSpeed)
                        zForceValue = 0;
                    else
                        zForceValue = CalculateAccelerationForce(accelerationInput.Value, axisProjectedVelocity.Value.z, MaxSpeed);
                } else {
                    if (axisProjectedVelocity.Value.z < MaxSpeedBackwards)
                        zForceValue = 0;
                    else 
                        zForceValue = CalculateAccelerationForce(accelerationInput.Value, axisProjectedVelocity.Value.z, MaxSpeedBackwards);
                }

                float3 zForceVector = forceCastForward * zForceValue;

                ecb.SetComponent(wheel, new WheelAxisForceAcceleration { Value = zForceVector });
            }
        }

        private float CalculateAccelerationForce(float accelerationInput, float velocityZ, float maxSpeed) {
            float zForce;
            if (velocityZ * accelerationInput > 0)
                zForce = CalculateEngineForce(accelerationInput, velocityZ, maxSpeed);
            else
                zForce = CalculateEngineForce(accelerationInput, HardBreakingEngineCoefficient, maxSpeed) * HardBreakingForceMultiplier;

            return zForce;
        }

        private float CalculateEngineForce(float accelerationInput, float velocity, float maxSpeed) =>
            accelerationInput * (-.5f * (velocity / maxSpeed) + 1f);
    }
}