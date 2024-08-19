using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Infrastructure.Destruction
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    [UpdateBefore(typeof(DestroyEntitySystem))]
    public partial struct SelfDestructionSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (timeLeft, entity)
                in SystemAPI.Query<RefRW<SelfDestructTimeLeft>>()
                .WithEntityAccess()) {

                timeLeft.ValueRW.Value -= SystemAPI.Time.DeltaTime;

                if (timeLeft.ValueRW.Value > 0)
                    continue;

                ecb.AddComponent<DestroyEntityTag>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    public partial struct ClientSelfDestructionSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new(Unity.Collections.Allocator.Temp);

            foreach (var (timeLeft, entity)
                in SystemAPI.Query<RefRW<ClientSelfDestructTimeLeft>>()
                .WithEntityAccess()) {

                timeLeft.ValueRW.Value -= SystemAPI.Time.DeltaTime;

                if (timeLeft.ValueRW.Value > 0)
                    continue;

                ecb.DestroyEntity(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
