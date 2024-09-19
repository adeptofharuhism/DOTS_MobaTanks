using Unity.Burst;
using Unity.Entities;

namespace Assets.CodeBase.Destruction
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(DestructionSystemGroup), OrderLast = true)]
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
