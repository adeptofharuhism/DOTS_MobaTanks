using Assets.CodeBase.GameStates;
using Assets.CodeBase.Targeting;
using Unity.Burst;
using Unity.Entities;

namespace Assets.CodeBase.Mobs.Logic.TargetSearch
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(LogicSystemGroup), OrderLast = true)]
    public partial struct RemoveReadyToSearchTagSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (tag, entity)
                in SystemAPI.Query<MobReadyToSearchTargetTag>()
                .WithEntityAccess()) {

                ecb.RemoveComponent<Targeter>(entity);
                ecb.RemoveComponent<MobReadyToSearchTargetTag>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
