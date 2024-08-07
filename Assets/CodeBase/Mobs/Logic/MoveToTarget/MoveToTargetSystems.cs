﻿using Assets.CodeBase.GameStates;
using Assets.CodeBase.Mobs.Logic.Attack;
using Assets.CodeBase.Targeting;
using ProjectDawn.Navigation;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Assets.CodeBase.Mobs.Logic.MoveToTarget
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(MoveToTargetStateSystemGroup))]
    [UpdateBefore(typeof(EnterMoveToPointWhenNoTargetSystem))]
    public partial struct MoveToTargetStateInitializationSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (tag, entity)
                in SystemAPI.Query<EnterMoveToTargetState>()
                .WithEntityAccess()) {

                ecb.SetComponentEnabled<EnterMoveToTargetState>(entity,false);

                ecb.SetComponentEnabled<MoveToTargetState>(entity, true);
                ecb.SetComponentEnabled<SearchForNewTargetTag>(entity, true );
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(MoveToTargetStateSystemGroup))]
    [UpdateAfter(typeof(MoveToTargetStateInitializationSystem))]
    [UpdateBefore(typeof(UpdateChasedTargetSystem))]
    public partial struct EnterMoveToPointWhenNoTargetSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (currentTarget, chasedTarget, entity)
                in SystemAPI.Query<CurrentTarget, ChasedTarget>()
                .WithAll<MoveToTargetState>()
                .WithEntityAccess()) {

                if (state.EntityManager.Exists(currentTarget.Value) || state.EntityManager.Exists(chasedTarget.Value))
                    continue;

                ecb.SetComponentEnabled<HasTargetInRangeTag>(entity,false);
                ecb.SetComponentEnabled<SearchForNewTargetTag>(entity, false);
                ecb.SetComponentEnabled<MoveToTargetState>(entity, false);

                ecb.SetComponentEnabled<EnterMoveToPointState>(entity,true);
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(MoveToTargetStateSystemGroup))]
    [UpdateAfter(typeof(EnterMoveToPointWhenNoTargetSystem))]
    [UpdateBefore(typeof(UpdateChasedTargetPositionSystem))]
    public partial struct UpdateChasedTargetSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (currentTarget, chasedTarget, entity)
                in SystemAPI.Query<CurrentTarget, RefRW<ChasedTarget>>()
                .WithAll<MoveToTargetState, SearchForNewTargetTag>()
                .WithEntityAccess()) {

                if (!state.EntityManager.Exists(currentTarget.Value))
                    continue;

                chasedTarget.ValueRW.Value = currentTarget.Value;

                ecb.SetComponentEnabled<SearchForNewTargetTag>(entity, false);
                ecb.SetComponentEnabled<HasTargetInRangeTag>(entity, true);
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(MoveToTargetStateSystemGroup))]
    [UpdateAfter(typeof(UpdateChasedTargetSystem))]
    [UpdateBefore(typeof(FollowChasedTargetSystem))]
    public partial struct UpdateChasedTargetPositionSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (chasedTarget, chasedTargetPosition)
                in SystemAPI.Query<ChasedTarget, RefRW<ChasedTargetPosition>>()
                .WithAll<MoveToTargetState>()) {

                if (!SystemAPI.GetComponentLookup<LocalTransform>(true)
                    .TryGetComponent(chasedTarget.Value, out LocalTransform targetTransform))
                    continue;

                chasedTargetPosition.ValueRW.Value = targetTransform.Position;
            }
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(MoveToTargetStateSystemGroup))]
    [UpdateAfter(typeof(UpdateChasedTargetPositionSystem))]
    [UpdateBefore(typeof(CalculateSquaredDistanceToChasedTargetSystem))]
    public partial struct FollowChasedTargetSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (targetPosition, agent)
                in SystemAPI.Query<ChasedTargetPosition, RefRW<AgentBody>>()
                .WithAll<MoveToTargetState>()) {

                agent.ValueRW.SetDestination(targetPosition.Value);
            }
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(MoveToTargetStateSystemGroup))]
    [UpdateAfter(typeof(FollowChasedTargetSystem))]
    [UpdateBefore(typeof(EnterAttackWhenTargetInRangeSystem))]
    public partial struct CalculateSquaredDistanceToChasedTargetSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (targetPosition, squaredDistance, transform)
                in SystemAPI.Query<ChasedTargetPosition, RefRW<SquaredChasedTargetDistance>, LocalToWorld>()
                .WithAll<MoveToTargetState>()) {

                squaredDistance.ValueRW.Value = math.distancesq(targetPosition.Value, transform.Position);
            }
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(MoveToTargetStateSystemGroup))]
    [UpdateAfter(typeof(CalculateSquaredDistanceToChasedTargetSystem))]
    [UpdateBefore(typeof(CheckIfTargetInRangeSystem))]
    public partial struct EnterAttackWhenTargetInRangeSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (attackDistance, targetDistance, entity)
                in SystemAPI.Query<SquaredAttackDistance, SquaredChasedTargetDistance>()
                .WithAll<MoveToTargetState, HasTargetInRangeTag>()
                .WithEntityAccess()) {

                if (targetDistance.Value > attackDistance.Value)
                    continue;

                ecb.SetComponentEnabled<HasTargetInRangeTag>(entity, false);
                ecb.SetComponentEnabled<SearchForNewTargetTag>(entity, false);
                ecb.SetComponentEnabled<MoveToTargetState>(entity, false);

                ecb.SetComponentEnabled<EnterAttackState>(entity, true);
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(MoveToTargetStateSystemGroup))]
    [UpdateAfter(typeof(EnterAttackWhenTargetInRangeSystem))]
    [UpdateBefore(typeof(UpdateChaseTimeLeftSystem))]
    public partial struct CheckIfTargetInRangeSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (targetDistance, searchRange, chaseDuration, entity)
                in SystemAPI.Query<SquaredChasedTargetDistance, SquaredTargetSearchRange, ChaseDuration>()
                .WithAll<MoveToTargetState, HasTargetInRangeTag>()
                .WithEntityAccess()) {

                if (targetDistance.Value < searchRange.Value)
                    continue;

                ecb.SetComponentEnabled<SearchForNewTargetTag>(entity, true);
                ecb.SetComponentEnabled<HasTargetInRangeTag>(entity, false);

                ecb.SetComponent(entity, new ChaseTimeLeft { Value = chaseDuration.Value });
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(MoveToTargetStateSystemGroup))]
    [UpdateAfter(typeof(CheckIfTargetInRangeSystem))]
    [UpdateBefore(typeof(EnterMoveToPointWhenTargetChaseOverSystem))]
    public partial struct UpdateChaseTimeLeftSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var timeLeft
                in SystemAPI.Query<RefRW<ChaseTimeLeft>>()
                .WithAll<MoveToTargetState, SearchForNewTargetTag>()) {

                timeLeft.ValueRW.Value -= SystemAPI.Time.DeltaTime;
            }
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(MoveToTargetStateSystemGroup))]
    [UpdateAfter(typeof(UpdateChaseTimeLeftSystem))]
    public partial struct EnterMoveToPointWhenTargetChaseOverSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (timeLeft, entity)
                in SystemAPI.Query<ChaseTimeLeft>()
                .WithAll<MoveToTargetState, SearchForNewTargetTag>()
                .WithEntityAccess()) {

                if (timeLeft.Value > 0)
                    continue;

                ecb.SetComponentEnabled<HasTargetInRangeTag>(entity, false);
                ecb.SetComponentEnabled<SearchForNewTargetTag>(entity, false);
                ecb.SetComponentEnabled<MoveToTargetState>(entity, false);

                ecb.SetComponentEnabled<EnterMoveToPointState>(entity, true);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
