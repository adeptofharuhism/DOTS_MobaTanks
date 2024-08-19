using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace Assets.CodeBase.Vehicles.Wheels
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(WheelLookForGroundContactSystem))]
    public partial struct WheelSteeringForceCalculationSystem : ISystem
    {
        private const float Epsilon = 1E-06f;

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (steeringParameters, axisProjectedVelocity, forceCastPoint, wheel)
                in SystemAPI.Query<WheelSteeringParameters, WheelAxisProjectedVelocity, WheelForceCastPoint>()
                .WithAll<WheelInitializedTag, WheelHasGroundContactTag>()
                .WithEntityAccess()) {

                RefRO<LocalToWorld> forceCastTransform = SystemAPI.GetComponentRO<LocalToWorld>(forceCastPoint.Value);

                ecb.SetComponent(wheel, new WheelAxisForceSteering {
                    Value = CalculateXVector(
                        forceCastTransform.ValueRO.Right,
                        steeringParameters.MaximalSteering,
                        steeringParameters.MinimalSteering,
                        axisProjectedVelocity.Value.x,
                        axisProjectedVelocity.Value.z)
                });
            }
        }

        private float3 CalculateXVector(float3 forceCastRight, float maxSteering, float minSteering, float velocityX, float velocityZ) =>
            -forceCastRight * CalculateXForce(maxSteering, minSteering, velocityX, velocityZ);

        private float CalculateXForce(float maxSteering, float minSteering, float velocityX, float velocityZ) =>
            CalculateTractionCoefficient(maxSteering, minSteering, velocityX, velocityZ) * velocityX;

        private float CalculateTractionCoefficient(float maxSteering, float minSteering, float velocityX, float velocityZ) =>
            math.lerp(maxSteering, minSteering, CalculateXVelocityToHorizontalVelocityRatio(velocityX, velocityZ));

        private float CalculateXVelocityToHorizontalVelocityRatio(float velocityX, float velocityZ) =>
            math.square(velocityX) / (Epsilon + math.square(velocityX) + math.square(velocityZ));
    }
}