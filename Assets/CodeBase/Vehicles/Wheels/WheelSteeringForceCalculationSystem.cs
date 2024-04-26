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

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (steeringParameters, axisProjectedVelocity, forceCastPoint, wheel)
                in SystemAPI.Query<WheelSteeringParameters, WheelAxisProjectedVelocity, WheelForceCastPoint>()
                .WithAll<WheelInitializedTag, WheelHasGroundContactTag>()
                .WithEntityAccess()) {

                RefRO<LocalToWorld> forceCastTransform = SystemAPI.GetComponentRO<LocalToWorld>(forceCastPoint.Value);

                float xAxisToHorizontalVelocityRatio =
                    math.square(axisProjectedVelocity.Value.x)
                    / (Epsilon + math.square(axisProjectedVelocity.Value.x) + math.square(axisProjectedVelocity.Value.z));

                float tractionCoefficient = 
                    math.lerp(steeringParameters.MaximalSteering, steeringParameters.MinimalSteering, xAxisToHorizontalVelocityRatio);

                float xForceValue = tractionCoefficient * axisProjectedVelocity.Value.x;

                float3 xForceVector = -forceCastTransform.ValueRO.Right * xForceValue;

                ecb.SetComponent(wheel, new WheelAxisForceSteering { Value = xForceVector });
            }
        }
    }
}