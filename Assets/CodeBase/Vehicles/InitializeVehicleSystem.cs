using Unity.Burst;
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
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (newVehicleTag, newVehicle)
                in SystemAPI.Query<NewVehicleTag>()
                .WithEntityAccess()) {

                ecb.RemoveComponent<NewVehicleTag>(newVehicle);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}