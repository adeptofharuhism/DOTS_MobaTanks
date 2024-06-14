using Assets.CodeBase.Infrastructure.Destruction;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace Assets.CodeBase.Weapon.Projectile
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct ProjectileCollisionDetectionSystem : ISystem
    {
        private CollisionFilter _collisionFilter;

        public void OnCreate(ref SystemState state) {
            _collisionFilter = new CollisionFilter {
                BelongsTo = 1 << 6,
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

            foreach (var (speed, transform, projectile)
                in SystemAPI.Query<ProjectileSpeed, RefRO<LocalToWorld>>()
                .WithEntityAccess()) {

                RaycastInput raycastInput = new RaycastInput {
                    Start = transform.ValueRO.Position,
                    End = transform.ValueRO.Position + transform.ValueRO.Forward * speed.Value * SystemAPI.Time.DeltaTime,
                    Filter = _collisionFilter
                };

                NativeList<RaycastHit> hitList = new NativeList<RaycastHit>(Allocator.Temp);
                bool hasHit = collisionWorld.CastRay(raycastInput, ref hitList);

                if (hasHit) {
                    ecb.AddComponent<DestroyEntityTag>(projectile);
                }
            }
        }
    }
}