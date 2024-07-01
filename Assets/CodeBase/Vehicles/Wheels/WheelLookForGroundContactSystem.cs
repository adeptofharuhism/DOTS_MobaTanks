using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace Assets.CodeBase.Vehicles.Wheels
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(WheelCalculateLinearVelocitySystem))]
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

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            CollisionWorld collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
            EntityCommandBuffer ecb =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (forceCastPoint, springRestDistance, wheel)
                in SystemAPI.Query<WheelForceCastPoint, WheelSpringRestDistance>()
                .WithAll<WheelInitializedTag>()
                .WithEntityAccess()) {

                RefRO<LocalToWorld> forceCastTransform = SystemAPI.GetComponentRO<LocalToWorld>(forceCastPoint.Value);

                RaycastInput raycastInput = new RaycastInput {
                    Start = forceCastTransform.ValueRO.Position,
                    End = forceCastTransform.ValueRO.Position - math.normalize(forceCastTransform.ValueRO.Up) * springRestDistance.Value,
                    Filter = _collisionFilter
                };

                bool hasHit = collisionWorld.CastRay(raycastInput, out RaycastHit closestHit);
                if (hasHit)
                    ecb.AddComponent<WheelHasGroundContactTag>(wheel);
                else
                    ecb.RemoveComponent<WheelHasGroundContactTag>(wheel);

                float compressionCoefficient = hasHit ? closestHit.Fraction : 1;
                ecb.SetComponent(wheel, new WheelSpringCompression { 
                    Value = CalculateSpringCompression(springRestDistance.Value, compressionCoefficient)
                });
            }
        }

        [BurstCompile]
        private float CalculateSpringCompression(float springRestDistance, float compressionCoefficient) =>
            springRestDistance * (1 - compressionCoefficient);
    }
}
