using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Infrastructure.Destruction
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    [UpdateBefore(typeof(DestroyEntitySystem))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct SelfDestructionSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (destructTime, currentTime, entity)
                in SystemAPI.Query<SelfDestructLifetime, RefRW<SelfDestructCurrentTime>>()
                .WithEntityAccess()) {

                currentTime.ValueRW.Value += SystemAPI.Time.DeltaTime;

                if (currentTime.ValueRW.Value > destructTime.Value)
                    ecb.AddComponent<DestroyEntityTag>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
