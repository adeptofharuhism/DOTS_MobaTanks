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
        private const float MinimalTraction = .1f;
        private const float MaximalTraction = .5f;

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (axisProjectedVelocity, forceCastPoint, wheel)
                in SystemAPI.Query<WheelAxisProjectedVelocity, WheelForceCastPoint>()
                .WithAll<WheelInitializedTag, WheelHasGroundContactTag>()
                .WithEntityAccess()) {

                RefRO<LocalToWorld> forceCastTransform = SystemAPI.GetComponentRO<LocalToWorld>(forceCastPoint.Value);

                float xAxisToHorizontalVelocityRatio =
                    math.square(axisProjectedVelocity.Value.x)
                    / (Epsilon + math.square(axisProjectedVelocity.Value.x) + math.square(axisProjectedVelocity.Value.z));
                float tractionCoefficient = math.lerp(MinimalTraction, MaximalTraction, xAxisToHorizontalVelocityRatio);

                float xForceValue = tractionCoefficient * axisProjectedVelocity.Value.x * SystemAPI.Time.DeltaTime;

                float3 xForceVector = -forceCastTransform.ValueRO.Right * xForceValue;

                ecb.SetComponent(wheel, new WheelAxisForceSteering { Value = xForceVector });
            }
        }
    }
}