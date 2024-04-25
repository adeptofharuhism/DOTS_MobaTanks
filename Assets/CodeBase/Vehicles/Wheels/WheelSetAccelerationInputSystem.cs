using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Vehicles.Wheels
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct WheelSetAccelerationInputSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (parent, wheel)
                in SystemAPI.Query<WheelParent>()
                .WithAll<Simulate, WheelInitializedTag, WheelHasAccelerationTag>()
                .WithEntityAccess()) {

                RefRO<VehicleMovementInput> movementInput = SystemAPI.GetComponentRO<VehicleMovementInput>(parent.Value);

                ecb.SetComponent(wheel, new WheelAccelerationInput { Value = movementInput.ValueRO.Value.y });
            }

            ecb.Playback(state.EntityManager);
        }
    }
}