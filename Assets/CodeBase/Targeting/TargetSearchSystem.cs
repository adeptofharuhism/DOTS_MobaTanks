using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace Assets.CodeBase.Targeting
{
    [UpdateInGroup(typeof(PhysicsSystemGroup), OrderLast = true)]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct TargetSearchSystem : ISystem
    {
        private CollisionFilter _collisionFilter;
        private Random _random;

        public void OnCreate(ref SystemState state) {
            _collisionFilter = new CollisionFilter {
                BelongsTo = 1 << 5,
                CollidesWith = 1 << 4
            };

            _random = Random.CreateFromIndex(42);

            state.RequireForUpdate<PhysicsWorldSingleton>();
        }

        public void OnUpdate(ref SystemState state) {
            CollisionWorld collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

            foreach (var (range, targeterTransform, currentTarget)
                in SystemAPI.Query<TargeterRange, RefRO<LocalToWorld>, RefRW<CurrentTarget>>()
                .WithAll<Targeter>()) {

                NativeList<DistanceHit> distanceHits = new NativeList<DistanceHit>(Allocator.Temp);

                collisionWorld.OverlapSphere(
                    targeterTransform.ValueRO.Position,
                    range.Value,
                    ref distanceHits,
                    _collisionFilter,
                    QueryInteraction.IgnoreTriggers);

                if (distanceHits.Length == 0)
                    currentTarget.ValueRW.Value = Entity.Null;
                else {
                    int targetIndex = _random.NextInt(distanceHits.Length - 1);
                    currentTarget.ValueRW.Value = distanceHits[targetIndex].Entity;
                }
            }
        }
    }
}
