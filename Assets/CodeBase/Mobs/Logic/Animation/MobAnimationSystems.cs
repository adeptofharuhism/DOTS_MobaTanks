using AnimCooker;
using Assets.CodeBase.GameStates;
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
                .WithAny<EnterAttackState>()) {

                isMoving.ValueRW.Value = false;
            }
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(PostAttackStateSystemGroup))]
    public partial struct ActivateRunAnimationSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            foreach (var isMoving
                in SystemAPI.Query<RefRW<IsMoving>>()
                .WithAny<EnterMoveToPointState, EnterMoveToTargetState>()) {

                isMoving.ValueRW.Value = true;
            }
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(LogicSystemGroup))]
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
}
