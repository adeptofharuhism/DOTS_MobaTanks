using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace Assets.CodeBase.Vehicles.Wheels
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(WheelCalculateLinearVelocity))]
    public partial struct WheelLookForGroundContactSystem : ISystem
    {
        private CollisionFilter _collisionFilter;

        public void OnCreate(ref SystemState state) {
            _collisionFilter = new CollisionFilter {
                BelongsTo = 1 << 2,
                CollidesWith = 1 << 0 | 1 << 1 | 1 << 3
            };

            state.RequireForUpdate<PhysicsWorldSingleton>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state) {
            CollisionWorld collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
            EntityCommandBuffer ecb =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (forceCastPoint, springRestDistance, wheel)
                in SystemAPI.Query<WheelForceCastPoint, WheelSpringRestDistance>()
                .WithAll<WheelInitializedTag>()
                .WithEntityAccess()) {

                RefRO<LocalToWorld> transform = SystemAPI.GetComponentRO<LocalToWorld>(forceCastPoint.Value);

                RaycastInput raycastInput = new RaycastInput {
                    Start = transform.ValueRO.Position,
                    End = transform.ValueRO.Position - math.normalize(transform.ValueRO.Up) * springRestDistance.Value,
                    Filter = _collisionFilter
                };

                bool hasHit = collisionWorld.CastRay(raycastInput, out RaycastHit closestHit);
                if (hasHit)
                    ecb.AddComponent<WheelHasGroundContactTag>(wheel);
                else
                    ecb.RemoveComponent<WheelHasGroundContactTag>(wheel);

                float compressionCoefficient = hasHit ? closestHit.Fraction : 1;
                ecb.SetComponent(wheel, new WheelSpringCompression {
                    SpringLength = springRestDistance.Value * compressionCoefficient,
                    CompressionLength = springRestDistance.Value * (1 - compressionCoefficient)
                });
            }
        }
    }

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

            foreach (var (springCompression, linearVelocity, springStrength, forceCastPosition, parent)
                in SystemAPI.Query<WheelSpringCompression, WheelLinearVelocity, WheelSpringStrength, WheelForceCastPoint, WheelParent>()
                  .WithAll<WheelInitializedTag, WheelHasGroundContactTag>()) {

                RefRO<LocalToWorld> forceCastTransform = SystemAPI.GetComponentRO<LocalToWorld>(forceCastPosition.Value);

                float3 forceCastUp = forceCastTransform.ValueRO.Up;

                float yAxisVelocity = math.dot(forceCastUp, linearVelocity.Value);
                float yForceValue =
                    (springStrength.Strength * springCompression.CompressionLength)
                    - (springStrength.Damper * yAxisVelocity);

                float3 yForceVector = forceCastUp * yForceValue * SystemAPI.Time.DeltaTime;

                RefRO<LocalToWorld> parentTransform = SystemAPI.GetComponentRO<LocalToWorld>(parent.Value);

                Unity.Physics.Extensions.PhysicsComponentExtensions.ApplyImpulse(
                    ref SystemAPI.GetComponentRW<PhysicsVelocity>(parent.Value).ValueRW,
                    SystemAPI.GetComponentRW<PhysicsMass>(parent.Value).ValueRW,
                    parentTransform.ValueRO.Position,
                    parentTransform.ValueRO.Rotation,
                    yForceVector,
                    forceCastTransform.ValueRO.Position);
            }
        }
    }
}
