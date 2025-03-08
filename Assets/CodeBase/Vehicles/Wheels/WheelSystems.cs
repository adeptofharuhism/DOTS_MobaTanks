using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Assets.CodeBase.Vehicles.Wheels
{
    [UpdateInGroup(typeof(WheelSystemGroup), OrderFirst = true)]
    public partial struct InitializeWheelSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NewWheelTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (newWheel, wheel)
                in SystemAPI.Query<NewWheelTag>()
                .WithEntityAccess()) {

                ecb.AddComponent(wheel, new WheelParent { Value = GetHighestParentOfEntity(ref state, wheel) });

                ecb.RemoveComponent<NewWheelTag>(wheel);
                ecb.AddComponent<WheelInitializedTag>(wheel);
            }

            ecb.Playback(state.EntityManager);
        }

        private Entity GetHighestParentOfEntity(ref SystemState state, Entity entity) {
            bool hasParent = true;
            Entity parent = entity;
            while (hasParent) {
                if (SystemAPI.HasComponent<Parent>(parent)) {
                    parent = SystemAPI.GetComponent<Parent>(parent).Value;
                } else
                    hasParent = false;
            }

            return parent;
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(WheelSystemGroup))]
    [UpdateBefore(typeof(WheelClientRotationSystem))]
    public partial struct WheelClientModelDisplaySystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (parent, modelParameters, index)
                in SystemAPI.Query<WheelParent, WheelModelParameters, WheelIndex>()) {

                DynamicBuffer<VehicleSpringLengthCompressedBuffer> springBuffer =
                    SystemAPI.GetBuffer<VehicleSpringLengthCompressedBuffer>(parent.Value);

                foreach (var springInfo in springBuffer) {
                    if (springInfo.Index == index.Value) {

                        RefRW<LocalTransform> modelTransform =
                            SystemAPI.GetComponentRW<LocalTransform>(modelParameters.ModelContainer);
                        modelTransform.ValueRW.Position.y = modelParameters.Diameter - springInfo.Value;
                        break;
                    }
                }
            }
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(WheelSystemGroup))]
    [UpdateAfter(typeof(WheelClientModelDisplaySystem))]
    public partial struct WheelClientRotationSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (rotationParameters, forceCastPoint, parent)
                in SystemAPI.Query<WheelRotationParameters, WheelForceCastPoint, WheelParent>()) {

                RefRO<VehicleMovementInput> movementInput = SystemAPI.GetComponentRO<VehicleMovementInput>(parent.Value);

                RefRW<LocalTransform> forceCastTransform = SystemAPI.GetComponentRW<LocalTransform>(forceCastPoint.Value);

                forceCastTransform.ValueRW.Rotation =
                    CalculateRotationQuaternion(
                        movementInput.ValueRO.Value.x,
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
