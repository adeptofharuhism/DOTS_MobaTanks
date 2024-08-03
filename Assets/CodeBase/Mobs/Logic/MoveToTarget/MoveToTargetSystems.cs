using Assets.CodeBase.GameStates;
using Assets.CodeBase.Targeting;
using ProjectDawn.Navigation;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Assets.CodeBase.Mobs.Logic.MoveToTarget
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(MoveToTargetStateSystemGroup))]
    [UpdateBefore(typeof(NoTargetEnterMoveToPointSystem))]
    public partial struct MoveToTargetStateInitializationSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (tag, entity)
                in SystemAPI.Query<EnterMoveToTargetState>()
                .WithEntityAccess()) {

                ecb.RemoveComponent<EnterMoveToTargetState>(entity);

                ecb.AddComponent<MoveToTargetState>(entity);
                ecb.AddComponent<SearchForNewTargetTag>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(MoveToTargetStateSystemGroup))]
    [UpdateAfter(typeof(MoveToTargetStateInitializationSystem))]
    [UpdateBefore(typeof(UpdateChasedTargetSystem))]
    public partial struct NoTargetEnterMoveToPointSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (currentTarget, chasedTarget, entity)
                in SystemAPI.Query<CurrentTarget, ChasedTarget>()
                .WithEntityAccess()) {

                if (state.EntityManager.Exists(currentTarget.Value) || state.EntityManager.Exists(chasedTarget.Value))
                    continue;

                ecb.RemoveComponent<HasTargetInRangeTag>(entity);
                ecb.RemoveComponent<SearchForNewTargetTag>(entity);
                ecb.RemoveComponent<MoveToTargetState>(entity);

                ecb.AddComponent<EnterMoveToPointState>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(MoveToTargetStateSystemGroup))]
    [UpdateAfter(typeof(NoTargetEnterMoveToPointSystem))]
    [UpdateBefore(typeof(UpdateChasedTargetPositionSystem))]
    public partial struct UpdateChasedTargetSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (currentTarget, chasedTarget, entity)
                in SystemAPI.Query<CurrentTarget, RefRW<ChasedTarget>>()
                .WithAll<MoveToTargetState, SearchForNewTargetTag>()
                .WithEntityAccess()) {

                if (!state.EntityManager.Exists(currentTarget.Value))
                    continue;

                chasedTarget.ValueRW.Value = currentTarget.Value;

                ecb.RemoveComponent<SearchForNewTargetTag>(entity);
                ecb.AddComponent<HasTargetInRangeTag>(entity);
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

        public void OnUpdate(ref SystemState state) {
            foreach (var (chasedTarget, chasedTargetPosition)
                in SystemAPI.Query<ChasedTarget, RefRW<ChasedTargetPosition>>()
                .WithAll<MoveToTargetState>()) {

                RefRO<LocalToWorld> targetTransform = SystemAPI.GetComponentRO<LocalToWorld>(chasedTarget.Value);

                chasedTargetPosition.ValueRW.Value = targetTransform.ValueRO.Position;
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
    [UpdateBefore(typeof(CheckIfTargetInRangeSystem))]
    public partial struct CalculateSquaredDistanceToChasedTargetSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

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
    [UpdateBefore(typeof(UpdateChaseTimeLeftSystem))]
    public partial struct CheckIfTargetInRangeSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (targetDistance, searchRange, chaseDuration, entity)
                in SystemAPI.Query<SquaredChasedTargetDistance, SquaredTargetSearchRange, ChaseDuration>()
                .WithAll<MoveToTargetState, HasTargetInRangeTag>()
                .WithEntityAccess()) {

                if (targetDistance.Value < searchRange.Value)
                    continue;

                ecb.AddComponent<SearchForNewTargetTag>(entity);
                ecb.RemoveComponent<HasTargetInRangeTag>(entity);

                ecb.SetComponent(entity, new ChaseTimeLeft { Value = chaseDuration.Value });
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(MoveToTargetStateSystemGroup))]
    [UpdateAfter(typeof(CheckIfTargetInRangeSystem))]
    [UpdateBefore(typeof(TargetChaseOverEnterMoveToPointStateSystem))]
    public partial struct UpdateChaseTimeLeftSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            foreach (var (duration, timeLeft)
                in SystemAPI.Query<ChaseDuration, RefRW<ChaseTimeLeft>>()
                .WithAll<MoveToTargetState, SearchForNewTargetTag>()) {

                timeLeft.ValueRW.Value -= SystemAPI.Time.DeltaTime;
            }
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(MoveToTargetStateSystemGroup))]
    [UpdateAfter(typeof(UpdateChaseTimeLeftSystem))]
    public partial struct TargetChaseOverEnterMoveToPointStateSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (timeLeft, entity)
                in SystemAPI.Query<ChaseTimeLeft>()
                .WithAll<MoveToTargetState, SearchForNewTargetTag>()
                .WithEntityAccess()) {

                if (timeLeft.Value > 0)
                    continue;

                ecb.RemoveComponent<HasTargetInRangeTag>(entity);
                ecb.RemoveComponent<SearchForNewTargetTag>(entity);
                ecb.RemoveComponent<MoveToTargetState>(entity);

                ecb.AddComponent<EnterMoveToPointState>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
