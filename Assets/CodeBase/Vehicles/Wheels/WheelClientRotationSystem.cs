using Unity.Entities;
using Unity.Transforms;

namespace Assets.CodeBase.Vehicles.Wheels
{
    [UpdateAfter(typeof(TransformSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct WheelClientRotationSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            foreach (var (forceCastPoint, index, parent)
                in SystemAPI.Query<WheelForceCastPoint, WheelIndex, WheelParent>()) {

                DynamicBuffer<VehicleRotationBuffer> rotationBuffer =
                    SystemAPI.GetBuffer<VehicleRotationBuffer>(parent.Value);

                foreach (var rotationInfo in rotationBuffer) {
                    if (rotationInfo.Index == index.Value) {

                        RefRW<LocalTransform> forceCastTransform = SystemAPI.GetComponentRW<LocalTransform>(forceCastPoint.Value);
                        forceCastTransform.ValueRW.Rotation = rotationInfo.Value;

                        break;
                    }
                }
            }
        }
    }
}
