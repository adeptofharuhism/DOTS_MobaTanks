using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace Assets.CodeBase.Vehicles
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct InitializeVehicleLocalOwnershipSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (Entity vehicle
                in SystemAPI.QueryBuilder()
                .WithAll<GhostOwnerIsLocal, NotOwnerVehicleTag>()
                .Build().ToEntityArray(Allocator.Temp)) {

                ecb.AddComponent<OwnerVehicleTag>(vehicle);
                ecb.RemoveComponent<NotOwnerVehicleTag>(vehicle);

                ecb.SetComponent(vehicle, new VehicleMovementInput { Value = new float2(0, 0) });
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
