using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace Assets.CodeBase.Vehicles.Wheels
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(WheelLookForGroundContactSystem))]
    public partial struct WheelBrakingForceCalculationSystem : ISystem
    {
        private const float Epsilon = .2f;
        private const float BrakingStrength = 5f;

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
                float zForceValue = braking * SystemAPI.Time.DeltaTime;
                float3 zForceVector = forceCastForward * zForceValue;

                ecb.SetComponent(wheel, new WheelAxisForceAcceleration { Value = zForceVector });
            }
        }

        private float CalculateBraking(float velocity) {
            if (velocity > Epsilon)
                return -BrakingStrength;

            if (velocity < -Epsilon)
                return BrakingStrength;

            return -velocity;
        }
    }
}