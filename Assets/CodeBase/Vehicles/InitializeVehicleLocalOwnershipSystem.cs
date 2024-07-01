using Unity.Burst;
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
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (notOwnerTag, vehicle)
                in SystemAPI.Query<NotOwnerVehicleTag>()
                .WithAll<GhostOwnerIsLocal>()
                .WithEntityAccess()) {

                ecb.AddComponent<OwnerVehicleTag>(vehicle);
                ecb.RemoveComponent<NotOwnerVehicleTag>(vehicle);

                ecb.SetComponent(vehicle, new VehicleMovementInput { Value = new float2(0, 0) });
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
