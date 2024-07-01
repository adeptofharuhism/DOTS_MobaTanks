using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Vehicles.Wheels
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct WheelSetRotationInputSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (parent, wheel)
                in SystemAPI.Query<WheelParent>()
                .WithAll<Simulate, WheelInitializedTag, WheelHasRotationTag>()
                .WithEntityAccess()) {

                RefRO<VehicleMovementInput> movementInput = SystemAPI.GetComponentRO<VehicleMovementInput>(parent.Value);

                ecb.SetComponent(wheel, new WheelRotationInput { Value = movementInput.ValueRO.Value.x });
            }

            ecb.Playback(state.EntityManager);
        }
    }
}