using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Vehicles.Wheels
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    [UpdateAfter(typeof(WheelRotationSystem))]
    public partial struct WheelRotationGhostPassSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            foreach (var (currentRotation, index, parent)
                in SystemAPI.Query<WheelRotationCurrent, WheelIndex, WheelParent>()
                .WithAll<WheelInitializedTag, WheelHasRotationTag>()) {

                DynamicBuffer<VehicleRotationBuffer> rotationBuffer =
                    SystemAPI.GetBuffer<VehicleRotationBuffer>(parent.Value);

                rotationBuffer.RemoveAt(0);

                rotationBuffer.Add(new VehicleRotationBuffer {
                    Index = index.Value,
                    Value = currentRotation.Value
                });
            }
        }
    }
}