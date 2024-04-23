using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace Assets.CodeBase.Vehicles.Wheels
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(WheelLookForGroundContactSystem))]
    public partial struct WheelSpringForceCalculationSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (springCompression, axisProjectedVelocity, springStrength, forceCastPosition, wheel)
                in SystemAPI.Query<WheelSpringCompression, WheelAxisProjectedVelocity, WheelSpringStrength, WheelForceCastPoint>()
                  .WithAll<WheelInitializedTag, WheelHasGroundContactTag>()
                  .WithEntityAccess()) {

                RefRO<LocalToWorld> forceCastTransform = SystemAPI.GetComponentRO<LocalToWorld>(forceCastPosition.Value);

                float3 forceCastUp = forceCastTransform.ValueRO.Up;

                float yForceValue =
                    (springStrength.Strength * springCompression.CompressionLength)
                    - (springStrength.Damper * axisProjectedVelocity.Value.y);

                float3 yForceVector = forceCastUp * yForceValue * SystemAPI.Time.DeltaTime;

                ecb.SetComponent(wheel, new WheelAxisForceSpring { Value = yForceVector });
            }
        }
    }

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

                float xForceValue = tractionCoefficient * axisProjectedVelocity.Value.x;

                float3 xForceVector = -forceCastTransform.ValueRO.Right * xForceValue;

                ecb.SetComponent(wheel, new WheelAxisForceSteering { Value = xForceVector });
            }
        }
    }

    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(WheelSpringForceCalculationSystem))]
    [UpdateAfter(typeof(WheelSteeringForceCalculationSystem))]
    public partial struct WheelForceApplySystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (springForce, steeringForce, accelerationForce, forceCastPoint, parent)
                in SystemAPI.Query<WheelAxisForceSpring, WheelAxisForceSteering, WheelAxisForceAcceleration, WheelForceCastPoint, WheelParent>()
                .WithAll<WheelInitializedTag, WheelHasGroundContactTag>()) {

                RefRO<LocalToWorld> forceCastTransform = SystemAPI.GetComponentRO<LocalToWorld>(forceCastPoint.Value);
                RefRO<LocalToWorld> parentTransform = SystemAPI.GetComponentRO<LocalToWorld>(parent.Value);
                RefRW<PhysicsVelocity> parentVelocity = SystemAPI.GetComponentRW<PhysicsVelocity>(parent.Value);
                RefRW<PhysicsMass> parentMass = SystemAPI.GetComponentRW<PhysicsMass>(parent.Value);

                float3 forceVector = springForce.Value + steeringForce.Value + accelerationForce.Value;

                Unity.Physics.Extensions.PhysicsComponentExtensions.ApplyImpulse(
                    ref parentVelocity.ValueRW,
                    parentMass.ValueRW,
                    parentTransform.ValueRO.Position,
                    parentTransform.ValueRO.Rotation,
                    forceVector,
                    forceCastTransform.ValueRO.Position);
            }
        }
    }
}
