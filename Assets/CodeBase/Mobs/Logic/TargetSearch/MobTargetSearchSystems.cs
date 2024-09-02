using Assets.CodeBase.GameStates;
using Assets.CodeBase.Targeting;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Assets.CodeBase.Mobs.Logic.TargetSearch
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(TargetingActivationSystemGroup))]
    public partial struct UpdateMobTargetSearchCooldownSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (cooldown, timePassed, entity)
                in SystemAPI.Query<TargetSearchCooldown, RefRW<TargetSearchCooldownTimeLeft>>()
                .WithEntityAccess()) {

                timePassed.ValueRW.Value -= SystemAPI.Time.DeltaTime;

                if (timePassed.ValueRO.Value > 0)
                    continue;

                timePassed.ValueRW.Value = cooldown.Value;

                ecb.SetComponentEnabled<Targeter>(entity, true);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
