using Assets.CodeBase.Combat.Health;
using Assets.CodeBase.Infrastructure.Destruction;
using Unity.Collections;
using Unity.Entities;
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
        }

        public void OnUpdate(ref SystemState state) {
            CollisionWorld collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (speed, damage, transform, projectile)
                in SystemAPI.Query<ProjectileSpeed, ProjectileDamage, RefRO<LocalToWorld>>()
                .WithEntityAccess()) {

                RaycastInput raycastInput = new RaycastInput {
                    Start = transform.ValueRO.Position,
                    End = transform.ValueRO.Position + transform.ValueRO.Forward * speed.Value * SystemAPI.Time.DeltaTime,
                    Filter = _collisionFilter
                };

                NativeList<RaycastHit> hitList = new NativeList<RaycastHit>(Allocator.Temp);
                bool hasHit = collisionWorld.CastRay(raycastInput, ref hitList);

                if (hasHit) {
                    foreach (RaycastHit hit in hitList) {
                        if (SystemAPI.HasBuffer<DamageBufferElement>(hit.Entity)) {
                            DynamicBuffer<DamageBufferElement> damageBuffer =
                                SystemAPI.GetBuffer<DamageBufferElement>(hit.Entity);

                            damageBuffer.Add(new DamageBufferElement { Value = damage.Value });

                            break;
                        }
                    }

                    ecb.AddComponent<DestroyEntityTag>(projectile);
                }
            }

            ecb.Playback(state.EntityManager);
        }
    }
}