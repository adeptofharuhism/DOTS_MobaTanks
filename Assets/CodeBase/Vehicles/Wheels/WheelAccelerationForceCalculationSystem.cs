using Unity.Burst;
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
        private const float HyperbolicOffsetY = 1.02f;
        private const float HyperbolaAngleMultiplier = 0.03f;
        private const float HyperbolicOffsetX = -1.03f;
        private const float HardBrakingVelocityValue = 0;

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (accelerationParameters, accelerationInput, axisProjectedVelocity, forceCastPosition, wheel)
                in SystemAPI.Query<WheelAccelerationParameters, WheelAccelerationInput, WheelAxisProjectedVelocity, WheelForceCastPoint>()
                .WithAll<WheelInitializedTag, WheelHasGroundContactTag, WheelAcceleratingTag>()
                .WithEntityAccess()) {

                RefRO<LocalToWorld> forceCastTransform = SystemAPI.GetComponentRO<LocalToWorld>(forceCastPosition.Value);

                float3 forceCastForward = forceCastTransform.ValueRO.Forward;

                float zForceValue = CalculateZForce(
                    accelerationInput.Value,
                    axisProjectedVelocity.Value.z,
                    accelerationParameters.MaxVelocity,
                    accelerationParameters.MaxVelocityBackwards,
                    accelerationParameters.HardBrakingForceMultiplier,
                    accelerationParameters.EngineForceMultiplier);

                float3 zForceVector = forceCastForward * zForceValue;

                ecb.SetComponent(wheel, new WheelAxisForceAcceleration { Value = zForceVector });
            }
        }

        private float CalculateZForce(
            float accelerationInput,
            float velocityZ,
            float maxVelocity,
            float maxVelocityBackwards,
            float hardBrakingForceMultiplier,
            float engineForceMultiplier) =>

            (accelerationInput > Epsilon)
                ? velocityZ > maxVelocity
                    ? 0
                    : CalculateAccelerationForce(
                            accelerationInput,
                            velocityZ,
                            maxVelocity,
                            hardBrakingForceMultiplier,
                            engineForceMultiplier)
                : velocityZ < maxVelocityBackwards
                    ? 0
                    : CalculateAccelerationForce(
                            accelerationInput,
                            velocityZ,
                            maxVelocityBackwards,
                            hardBrakingForceMultiplier,
                            engineForceMultiplier);

        private float CalculateAccelerationForce(
            float accelerationInput,
            float velocityZ,
            float maxVelocity,
            float hardBrakingForceMultiplier,
            float engineForceMultiplier) =>

            (velocityZ * accelerationInput > 0
                ? CalculateEngineForce(accelerationInput, velocityZ, maxVelocity)
                : CalculateEngineForce(accelerationInput, HardBrakingVelocityValue, maxVelocity)
                    * hardBrakingForceMultiplier
            ) * engineForceMultiplier;

        private float CalculateEngineForce(float accelerationInput, float velocity, float maxSpeed) =>
            accelerationInput * HyperbolicCurve(velocity / maxSpeed);

        private float HyperbolicCurve(float x) =>
            HyperbolicOffsetY + (HyperbolaAngleMultiplier / (x + HyperbolicOffsetX));
    }
}