using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace Assets.CodeBase.Vehicles.Wheels
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(PhysicsSimulationGroup))]
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
                .WithEntityAccess()) {

                RefRO<LocalToWorld> transform = SystemAPI.GetComponentRO<LocalToWorld>(forceCastPoint.Value);

                RaycastInput raycastInput = new RaycastInput {
                    Start = transform.ValueRO.Position,
                    End = transform.ValueRO.Position - math.normalize(transform.ValueRO.Up) * springRestDistance.Value,
                    Filter = _collisionFilter
                };

                bool hasHit = collisionWorld.CastRay(raycastInput, out RaycastHit closestHit);
                ecb.SetComponent(wheel, new WheelHasGroundContact { Value = hasHit });
                ecb.SetComponent(wheel, new WheelCompressedSpringLength {
                    Value = hasHit
                        ? closestHit.Fraction * springRestDistance.Value
                        : springRestDistance.Value
                });
            }
        }
    }

    [UpdateAfter(typeof(TransformSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct WheelModelDisplaySystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            foreach (var (compressedLength, modelParameters)
                in SystemAPI.Query<WheelCompressedSpringLength, WheelModelParameters>()) {

                RefRW<LocalTransform> modelTransform = SystemAPI.GetComponentRW<LocalTransform>(modelParameters.ModelContainer);
                modelTransform.ValueRW.Position.y = modelParameters.Diameter - compressedLength.Value;
            }
        }
    }
}
