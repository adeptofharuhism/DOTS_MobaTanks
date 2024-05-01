using Unity.Entities;
using Unity.Transforms;

namespace Assets.CodeBase.Vehicles.Wheels
{
    [UpdateAfter(typeof(TransformSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct WheelModelDisplaySystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            foreach (var (compressedLength, modelParameters)
                in SystemAPI.Query<WheelSpringLengthCompressed, WheelModelParameters>()) {

                RefRW<LocalTransform> modelTransform = SystemAPI.GetComponentRW<LocalTransform>(modelParameters.ModelContainer);
                modelTransform.ValueRW.Position.y = modelParameters.Diameter - compressedLength.Value;
            }
        }
    }
}
