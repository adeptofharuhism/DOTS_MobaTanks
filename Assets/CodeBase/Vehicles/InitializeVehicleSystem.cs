using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Assets.CodeBase.Vehicles
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial struct InitializeVehicleSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NewVehicleTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach(Entity vehicle
                in SystemAPI.QueryBuilder()
                .WithAll<NewVehicleTag>()
                .Build().ToEntityArray(Allocator.Temp))

                ecb.RemoveComponent<NewVehicleTag>(vehicle);

            ecb.Playback(state.EntityManager);
        }
    }
}