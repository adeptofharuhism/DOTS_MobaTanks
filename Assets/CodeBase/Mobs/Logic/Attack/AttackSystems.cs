using Assets.CodeBase.Combat.Health;
using Assets.CodeBase.Combat.Teams;
using Assets.CodeBase.GameStates;
using Assets.CodeBase.Mobs.Logic.MoveToTarget;
using Assets.CodeBase.Targeting;
using ProjectDawn.Navigation;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Assets.CodeBase.Mobs.Logic.Attack
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(AttackStateSystemGroup))]
    [UpdateBefore(typeof(AttackStateInitializationSystem))]
    public partial struct DisableAttackHappenedTagSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            foreach (Entity entity
                in SystemAPI.QueryBuilder()
                .WithAll<AttackHappenedThisFrameTag>()
                .Build().ToEntityArray(Allocator.Temp))
                state.EntityManager.SetComponentEnabled<AttackHappenedThisFrameTag>(entity, false);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(AttackStateSystemGroup))]
    [UpdateAfter(typeof(DisableAttackHappenedTagSystem))]
    [UpdateBefore(typeof(EnterMoveToPointWhenNoTargetSystem))]
    public partial struct AttackStateInitializationSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (agent, entity)
                 in SystemAPI.Query<RefRW<AgentBody>>()
                 .WithAll<EnterAttackState>()
                 .WithEntityAccess()) {

                agent.ValueRW.Stop();

                ecb.SetComponentEnabled<EnterAttackState>(entity, false);

                ecb.SetComponentEnabled<AttackState>(entity, true);
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(AttackStateSystemGroup))]
    [UpdateAfter(typeof(AttackStateInitializationSystem))]
    [UpdateBefore(typeof(UpdateChasedTargetPositionSystem))]
    public partial struct EnterMoveToPointWhenNoTargetSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (target, entity)
                in SystemAPI.Query<ChasedTarget>()
                .WithAll<AttackState>()
                .WithEntityAccess()) {

                if (state.EntityManager.Exists(target.Value))
                    continue;

                ecb.SetComponentEnabled<AttackState>(entity, false);

                ecb.SetComponentEnabled<EnterMoveToPointState>(entity, true);
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(AttackStateSystemGroup))]
    [UpdateAfter(typeof(EnterMoveToPointWhenNoTargetSystem))]
    [UpdateBefore(typeof(CalculateSquaredDistanceToChasedTargetSystem))]
    public partial struct UpdateChasedTargetPositionSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (chasedTarget, chasedTargetPosition)
                in SystemAPI.Query<ChasedTarget, RefRW<ChasedTargetPosition>>()
                .WithAll<AttackState>()) {

                if (!SystemAPI.GetComponentLookup<LocalTransform>(true)
                    .TryGetComponent(chasedTarget.Value, out LocalTransform targetTransform))
                    continue;

                chasedTargetPosition.ValueRW.Value = targetTransform.Position;
            }
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(AttackStateSystemGroup))]
    [UpdateAfter(typeof(UpdateChasedTargetPositionSystem))]
    [UpdateBefore(typeof(EnterMoveToTargetWhenTargetIsFar))]
    public partial struct CalculateSquaredDistanceToChasedTargetSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (targetPosition, squaredDistance, transform)
                in SystemAPI.Query<ChasedTargetPosition, RefRW<SquaredChasedTargetDistance>, LocalToWorld>()
                .WithAll<AttackState>()) {

                squaredDistance.ValueRW.Value = math.distancesq(targetPosition.Value, transform.Position);
            }
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(AttackStateSystemGroup))]
    [UpdateAfter(typeof(CalculateSquaredDistanceToChasedTargetSystem))]
    [UpdateBefore(typeof(RotateOnTargetSystem))]
    public partial struct EnterMoveToTargetWhenTargetIsFar : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (attackDistance, targetDistance, entity)
                in SystemAPI.Query<SquaredAttackDistance, SquaredChasedTargetDistance>()
                .WithAll<AttackState>()
                .WithEntityAccess()) {

                if (attackDistance.Value > targetDistance.Value)
                    continue;

                ecb.SetComponentEnabled<AttackState>(entity, false);

                ecb.SetComponentEnabled<EnterMoveToTargetState>(entity, true);
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(AttackStateSystemGroup))]
    [UpdateAfter(typeof(EnterMoveToTargetWhenTargetIsFar))]
    [UpdateBefore(typeof(MeleeAttackSystem))]
    public partial struct RotateOnTargetSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            foreach (var (transform, targetPosition)
                in SystemAPI.Query<RefRW<LocalTransform>, ChasedTargetPosition>()
                .WithAll<AttackState>()) {

                float3 targetPositionOnPlane = targetPosition.Value;
                targetPositionOnPlane.y = transform.ValueRO.Position.y;

                float3 targetDirection = targetPositionOnPlane - transform.ValueRO.Position;
                targetDirection = math.normalize(targetDirection);

                float rotationAngle = math.atan2(targetDirection.x, targetDirection.z);

                transform.ValueRW.Rotation = quaternion.Euler(0, rotationAngle, 0);
            }
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(AttackStateSystemGroup))]
    [UpdateAfter(typeof(RotateOnTargetSystem))]
    [UpdateBefore(typeof(UpdateAimPositionSystem))]
    public partial struct MeleeAttackSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (target, damage)
                in SystemAPI.Query<ChasedTarget, AttackDamage>()
                .WithAll<AttackState, AttackIsReadyTag, MeleeAttackerTag>())

                if (SystemAPI.HasBuffer<DamageBufferElement>(target.Value))
                    SystemAPI.GetBuffer<DamageBufferElement>(target.Value)
                        .Add(new DamageBufferElement { Value = damage.Value });
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(AttackStateSystemGroup))]
    [UpdateAfter(typeof(MeleeAttackSystem))]
    [UpdateBefore(typeof(ProjectileAttackSystem))]
    public partial struct UpdateAimPositionSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            foreach (var (target, aimPoint)
                in SystemAPI.Query<ChasedTarget, RefRW<ProjectileAimPosition>>()
                .WithAll<AttackState, AttackIsReadyTag, ProjectileAttackerTag>()) {

                if (SystemAPI.GetComponentLookup<TargetPoint>(true)
                    .TryGetComponent(target.Value,out TargetPoint targetPoint)) {

                    if (SystemAPI.GetComponentLookup<LocalToWorld>(true)
                        .TryGetComponent(targetPoint.Value, out LocalToWorld targetTransform))
                        aimPoint.ValueRW.Value = targetTransform.Position;

                } else 
                    if (SystemAPI.GetComponentLookup<LocalTransform>(true)
                        .TryGetComponent(target.Value, out LocalTransform targetLocalTransform))
                        aimPoint.ValueRW.Value = targetLocalTransform.Position;
            }
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(AttackStateSystemGroup))]
    [UpdateAfter(typeof(UpdateAimPositionSystem))]
    [UpdateBefore(typeof(PostAttacTagSwitchkSystem))]
    public partial struct ProjectileAttackSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (aimPosition, prefab, spawnPoint, team)
                in SystemAPI.Query<ProjectileAimPosition, ProjectilePrefab, ProjectileSpawnPoint, UnitTeam>()
                .WithAll<AttackState, AttackIsReadyTag, ProjectileAttackerTag>()) {

                RefRO<LocalToWorld> spawnPointTransform = SystemAPI.GetComponentRO<LocalToWorld>(spawnPoint.Value);

                LocalTransform projectileTransform =
                    LocalTransform.FromPositionRotation(
                        spawnPointTransform.ValueRO.Position,
                        quaternion.LookRotationSafe(
                            math.normalize(aimPosition.Value - spawnPointTransform.ValueRO.Position),
                            math.up()));

                Entity projectile = ecb.Instantiate(prefab.Value);

                ecb.SetComponent(projectile, projectileTransform);
                ecb.SetComponent(projectile, new UnitTeam { Value = team.Value });
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(AttackStateSystemGroup))]
    [UpdateAfter(typeof(ProjectileAttackSystem))]
    [UpdateBefore(typeof(UpdateAttackCooldownSystem))]
    public partial struct PostAttacTagSwitchkSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (Entity entity
                in SystemAPI.QueryBuilder()
                .WithAll<AttackState, AttackIsReadyTag>()
                .Build().ToEntityArray(Allocator.Temp)) {

                ecb.SetComponentEnabled<AttackIsReadyTag>(entity, false);

                ecb.SetComponentEnabled<AttackIsOnCooldownTag>(entity, true);
                ecb.SetComponentEnabled<AttackHappenedThisFrameTag>(entity, true);
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(AttackStateSystemGroup))]
    [UpdateAfter(typeof(PostAttacTagSwitchkSystem))]
    public partial struct UpdateAttackCooldownSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (cooldownTimeLeft, cooldown, entity)
                in SystemAPI.Query<RefRW<AttackCooldownTimeLeft>, AttackCooldown>()
                .WithAll<AttackIsOnCooldownTag>()
                .WithEntityAccess()) {

                cooldownTimeLeft.ValueRW.Value -= SystemAPI.Time.DeltaTime;

                if (cooldownTimeLeft.ValueRO.Value > 0)
                    continue;

                cooldownTimeLeft.ValueRW.Value = cooldown.Value;

                ecb.SetComponentEnabled<AttackIsReadyTag>(entity, true);
                ecb.SetComponentEnabled<AttackIsOnCooldownTag>(entity, false);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
