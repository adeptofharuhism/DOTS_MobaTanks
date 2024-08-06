using Assets.CodeBase.Combat.Health;
using Assets.CodeBase.GameStates;
using Assets.CodeBase.Mobs.Logic.MoveToTarget;
using ProjectDawn.Navigation;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Assets.CodeBase.Mobs.Logic.Attack
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(AttackStateSystemGroup))]
    [UpdateBefore(typeof(EnterMoveToPointWhenNoTargetSystem))]
    public partial struct AttackStateInitializationSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

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
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

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
    [UpdateBefore(typeof(AttackSystem))]
    public partial struct EnterMoveToTargetWhenTargetIsFar : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

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
    [UpdateBefore(typeof(UpdateAttackCooldownSystem))]
    public partial struct AttackSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (target, damage, entity)
                in SystemAPI.Query<ChasedTarget, AttackDamage>()
                .WithAll<AttackState, AttackIsReadyTag>()
                .WithEntityAccess()) {

                if (SystemAPI.HasBuffer<DamageBufferElement>(target.Value))
                    SystemAPI.GetBuffer<DamageBufferElement>(target.Value)
                        .Add(new DamageBufferElement { Value = damage.Value });

                ecb.SetComponentEnabled<AttackIsOnCooldownTag>(entity, true);
                ecb.SetComponentEnabled<AttackIsReadyTag>(entity, false);
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(AttackStateSystemGroup))]
    [UpdateAfter(typeof(AttackSystem))]
    public partial struct UpdateAttackCooldownSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

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
