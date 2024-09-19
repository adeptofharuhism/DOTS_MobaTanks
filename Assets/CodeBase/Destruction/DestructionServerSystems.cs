using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Assets.CodeBase.Destruction
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(DestructionSystemGroup))]
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

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(DestructionSystemGroup), OrderLast = true)]
    public partial struct DestroyEntitySystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (Entity entity
                in SystemAPI.QueryBuilder()
                .WithAll<DestroyEntityTag>()
                .Build().ToEntityArray(Allocator.Temp))

                ecb.DestroyEntity(entity);
        }
    }
}

