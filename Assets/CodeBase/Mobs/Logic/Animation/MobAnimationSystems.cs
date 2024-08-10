using AnimCooker;
using Assets.CodeBase.GameStates;
using Assets.CodeBase.Mobs.Logic.Attack;
using Unity.Entities;

namespace Assets.CodeBase.Mobs.Logic.Animation
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(PreAttackStateSystemGroup))]
    public partial struct ActivateIdleAnimationSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            foreach (var isMoving
                in SystemAPI.Query<RefRW<IsMoving>>()
                .WithAny<EnterAttackState>())

                isMoving.ValueRW.Value = false;
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(PostAttackStateSystemGroup))]
    [UpdateBefore(typeof(ActivateAttackAnimationSystem))]
    public partial struct ActivateRunAnimationSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            foreach (var isMoving
                in SystemAPI.Query<RefRW<IsMoving>>()
                .WithAny<EnterMoveToPointState, EnterMoveToTargetState>())

                isMoving.ValueRW.Value = true;
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(PostAttackStateSystemGroup))]
    [UpdateAfter(typeof(ActivateRunAnimationSystem))]
    public partial struct ActivateAttackAnimationSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            foreach (var attackFlag
                in SystemAPI.Query<RefRW<AttackFlag>>()
                .WithAll<AttackHappenedThisFrameTag>()) 

                attackFlag.ValueRW.Value = !attackFlag.ValueRO.Value;
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(LogicSystemGroup))]
    [UpdateBefore(typeof(SetAttackAnimationSystem))]
    public partial struct SetMovementAnimationSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            foreach (var (idleClipIndex, runClipIndex, isMoving, prevIsMoving, entity)
                in SystemAPI.Query<IdleClipIndex, RunClipIndex, IsMoving, RefRW<PreviousIsMoving>>()
                .WithEntityAccess()) {

                if (isMoving.Value == prevIsMoving.ValueRO.Value)
                    continue;

                prevIsMoving.ValueRW.Value = isMoving.Value;

                state.EntityManager.SetComponentData(entity, new AnimationCmdData {
                    ClipIndex = isMoving.Value ? runClipIndex.Value : idleClipIndex.Value,
                    Cmd = AnimationCmd.SetPlayForever
                });
            }
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(LogicSystemGroup))]
    [UpdateAfter(typeof(SetMovementAnimationSystem))]
    public partial struct SetAttackAnimationSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            foreach (var (clipIndex, isAttacking, prevIsAttacking, entity)
                in SystemAPI.Query<AttackClipIndex, AttackFlag, RefRW<PreviousIsAttacking>>()
                .WithEntityAccess()) {

                if (isAttacking.Value == prevIsAttacking.ValueRO.Value)
                    continue;

                prevIsAttacking.ValueRW.Value = isAttacking.Value;

                state.EntityManager.SetComponentData(entity, new AnimationCmdData {
                    ClipIndex = clipIndex.Value,
                    Cmd = AnimationCmd.PlayOnce
                });
            }
        }
    }
}
