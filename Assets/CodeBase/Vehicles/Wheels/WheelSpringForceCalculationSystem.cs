using Unity.Entities;
using Unity.Mathematics;
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

                //UnityEngine.Debug.Log($"{yForceValue}");
                //UnityEngine.Debug.DrawLine(forceCastTransform.ValueRO.Position, forceCastTransform.ValueRO.Position + forceCastTransform.ValueRO.Up * (springStrength.Strength * springCompression.CompressionLength), UnityEngine.Color.green);
                //UnityEngine.Debug.DrawLine(forceCastTransform.ValueRO.Position, forceCastTransform.ValueRO.Position + forceCastTransform.ValueRO.Up * (-(springStrength.Damper * axisProjectedVelocity.Value.y)), UnityEngine.Color.red);

                float3 yForceVector = forceCastUp * yForceValue * SystemAPI.Time.DeltaTime;

                ecb.SetComponent(wheel, new WheelAxisForceSpring { Value = yForceVector });
            }
        }
    }

    //[UpdateInGroup(typeof(GhostInputSystemGroup))]
    //[UpdateAfter(typeof(VehicleMovementInputSystem))]
    //public partial struct WheelSetAccelerationInputSystem : ISystem
    //{
    //    public void OnUpdate(ref SystemState state) {

    //    }
    //}

    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(WheelLookForGroundContactSystem))]
    public partial struct WheelAccelerationForceCalculationSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var v
                in SystemAPI.Query<WheelAxisProjectedVelocity>()) {


            }
        }
    }

    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(WheelLookForGroundContactSystem))]
    public partial struct WheelBrakingForceCalculationSystem : ISystem
    {
        private const float BrakingStrength = 1;

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (axisProjectedVelocity, forceCastPoint, wheel)
                in SystemAPI.Query<WheelAxisProjectedVelocity, WheelForceCastPoint>()
                .WithAll<WheelBrakingTag, WheelInitializedTag, WheelHasGroundContactTag>()
                .WithEntityAccess()) {

                RefRO<LocalToWorld> forceCastTransform = SystemAPI.GetComponentRO<LocalToWorld>(forceCastPoint.Value);

                float3 forceCastForward = forceCastTransform.ValueRO.Forward;

                float braking = CalculateBraking(axisProjectedVelocity.Value.z);
                float zForceValue = braking;// * SystemAPI.Time.DeltaTime;
                float3 zForceVector = forceCastForward * zForceValue;

                ecb.SetComponent(wheel, new WheelAxisForceAcceleration { Value = zForceVector });
            }
        }

        private float CalculateBraking(float velocity) {
            if (velocity > BrakingStrength)
                return -BrakingStrength;

            if (velocity < -BrakingStrength)
                return BrakingStrength;

            return -velocity;
        }
    }
}