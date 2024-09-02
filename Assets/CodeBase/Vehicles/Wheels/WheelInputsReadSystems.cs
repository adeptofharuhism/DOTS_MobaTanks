using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Assets.CodeBase.Vehicles.Wheels
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(WheelInputsReadSystemGroup))]
    public partial struct WheelSetAccelerationInputSystem : ISystem
    {
        [BurstCompile]
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

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(WheelInputsReadSystemGroup))]
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

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(WheelInputsReadSystemGroup))]
    [UpdateAfter(typeof(WheelSetRotationInputSystem))]
    public partial struct WheelRotationSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (rotationInput, rotationParameters, forceCastPoint)
                in SystemAPI.Query<WheelRotationInput, WheelRotationParameters, WheelForceCastPoint>()
                .WithAll<WheelHasRotationTag, WheelInitializedTag, Simulate>()) {

                RefRW<LocalTransform> forceCastTransform = SystemAPI.GetComponentRW<LocalTransform>(forceCastPoint.Value);

                forceCastTransform.ValueRW.Rotation =
                    CalculateRotationQuaternion(
                        rotationInput.Value,
                        rotationParameters.MaxRotationAngle,
                        rotationParameters.RotatesClockwise);
            }
        }

        private quaternion CalculateRotationQuaternion(float rotationInput, float maxRotationAngle, bool rotatesClockwise) =>
            quaternion.Euler(
                0,
                math.radians(CalculateRotationAngle(rotationInput, maxRotationAngle, rotatesClockwise)),
                0);

        private float CalculateRotationAngle(float rotationInput, float maxRotationAngle, bool rotatesClockwise) =>
            rotationInput * (rotatesClockwise ? 1 : -1) * maxRotationAngle;
    }
}
