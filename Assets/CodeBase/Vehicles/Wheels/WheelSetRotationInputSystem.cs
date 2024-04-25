using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace Assets.CodeBase.Vehicles.Wheels
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct WheelSetRotationInputSystem : ISystem
    {
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

    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    [UpdateAfter(typeof(WheelSetRotationInputSystem))]
    public partial struct WheelRotationSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            foreach (var (rotationInput, rotationParameters, forceCastPoint)
                in SystemAPI.Query<WheelRotationInput, WheelRotationParameters, WheelForceCastPoint>()
                .WithAll<WheelHasRotationTag, WheelInitializedTag, Simulate>()) {

                RefRW<LocalTransform> forceCastTransform = SystemAPI.GetComponentRW<LocalTransform>(forceCastPoint.Value);

                int clockwiseMultiplier = rotationParameters.RotatesClockwise ? 1 : -1;
                float rotationAngle = rotationInput.Value * clockwiseMultiplier * rotationParameters.MaxRotationAngle;

                forceCastTransform.ValueRW.Rotate(quaternion.Euler(0, rotationAngle, 0));
            }
        }
    }
}