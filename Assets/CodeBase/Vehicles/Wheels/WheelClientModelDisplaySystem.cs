using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Assets.CodeBase.Vehicles.Wheels
{
    [UpdateAfter(typeof(TransformSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct WheelClientModelDisplaySystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (parent, modelParameters, index)
                in SystemAPI.Query<WheelParent, WheelModelParameters, WheelIndex>()) {

                DynamicBuffer<VehicleSpringLengthCompressedBuffer> springBuffer =
                    SystemAPI.GetBuffer<VehicleSpringLengthCompressedBuffer>(parent.Value);

                foreach(var springInfo in springBuffer) { 
                    if (springInfo.Index == index.Value) {

                        RefRW<LocalTransform> modelTransform = SystemAPI.GetComponentRW<LocalTransform>(modelParameters.ModelContainer);
                        modelTransform.ValueRW.Position.y = modelParameters.Diameter - springInfo.Value;
                        break;
                    }
                }
            }
        }
    }
}
