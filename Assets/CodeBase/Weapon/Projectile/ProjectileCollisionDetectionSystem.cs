using Assets.CodeBase.Combat.Health;
using Assets.CodeBase.Combat.Teams;
using Assets.CodeBase.Infrastructure.Destruction;
using Unity.Burst;
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

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            CollisionWorld collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (speed, damage, transform, team, projectile)
                in SystemAPI.Query<ProjectileSpeed, ProjectileDamage, RefRO<LocalToWorld>, UnitTeam>()
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
                        UnitTeam hitTeam = GetUnitTeam(ref state, hit.Entity);

                        if (hitTeam.Value == team.Value)
                            continue;

                        TryDoDamage(ref state, hit.Entity, damage.Value);

                        ecb.AddComponent<DestroyEntityTag>(projectile);
                        break;
                    }
                }
            }

            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        private UnitTeam GetUnitTeam(ref SystemState state, Entity entity) {
            UnitTeam team;

            bool hasTeam = SystemAPI.HasComponent<UnitTeam>(entity);
            if (hasTeam)
                team = SystemAPI.GetComponent<UnitTeam>(entity);
            else team = new UnitTeam { Value = TeamType.None };

            return team;
        }

        [BurstCompile]
        private void TryDoDamage(ref SystemState state, Entity entity, float damage) {
            if (SystemAPI.HasBuffer<DamageBufferElement>(entity)) {
                DynamicBuffer<DamageBufferElement> damageBuffer = SystemAPI.GetBuffer<DamageBufferElement>(entity);

                damageBuffer.Add(new DamageBufferElement { Value = damage });
            }
        }
    }
}