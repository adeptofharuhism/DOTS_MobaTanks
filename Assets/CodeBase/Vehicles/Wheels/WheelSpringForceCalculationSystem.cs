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
                    (springStrength.Strength * springCompression.Value)
                    - (springStrength.Damper * axisProjectedVelocity.Value.y);

                float3 yForceVector = forceCastUp * yForceValue  * SystemAPI.Time.DeltaTime;

                ecb.SetComponent(wheel, new WheelAxisForceSpring { Value = yForceVector });
            }
        }
    }
}