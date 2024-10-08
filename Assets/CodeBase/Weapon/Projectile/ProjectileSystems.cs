﻿using Assets.CodeBase.Combat.Health;
using Assets.CodeBase.Destruction;
using Assets.CodeBase.Teams;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace Assets.CodeBase.Weapon.Projectile
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(ProjectileSystemGroup))]
    [UpdateBefore(typeof(ProjectileMoveSystem))]
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
                in SystemAPI.Query<ProjectileSpeed, ProjectileDamage, RefRO<LocalTransform>, UnitTeam>()
                .WithEntityAccess()) {

                RaycastInput raycastInput = new RaycastInput {
                    Start = transform.ValueRO.Position,
                    End = transform.ValueRO.Position + transform.ValueRO.Forward() * speed.Value * SystemAPI.Time.DeltaTime,
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

        private UnitTeam GetUnitTeam(ref SystemState state, Entity entity) {
            UnitTeam team;

            bool hasTeam = SystemAPI.HasComponent<UnitTeam>(entity);
            if (hasTeam)
                team = SystemAPI.GetComponent<UnitTeam>(entity);
            else team = new UnitTeam { Value = TeamType.None };

            return team;
        }

        private void TryDoDamage(ref SystemState state, Entity entity, float damage) {
            if (SystemAPI.HasBuffer<DamageBufferElement>(entity)) {
                DynamicBuffer<DamageBufferElement> damageBuffer = SystemAPI.GetBuffer<DamageBufferElement>(entity);

                damageBuffer.Add(new DamageBufferElement { Value = damage });
            }
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(ProjectileSystemGroup))]
    [UpdateAfter(typeof(ProjectileCollisionDetectionSystem))]
    public partial struct ProjectileMoveSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (transform, speed)
                in SystemAPI.Query<RefRW<LocalTransform>, ProjectileSpeed>()) {

                transform.ValueRW.Position += transform.ValueRW.Forward() * speed.Value * SystemAPI.Time.DeltaTime;
            }
        }
    }
}