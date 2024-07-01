using Unity.Burst;
using Unity.Entities;
using Unity.Physics.Systems;

namespace Assets.CodeBase.Vehicles.Wheels
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(WheelLookForGroundContactSystem))]
    public partial struct WheelSpringLengthGhostPassSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (springCompression, springRestDistance, parent, index)
                in SystemAPI.Query<WheelSpringCompression, WheelSpringRestDistance, WheelParent, WheelIndex>()
                .WithAll<WheelInitializedTag>()) {

                DynamicBuffer<VehicleSpringLengthCompressedBuffer> springBuffer =
                    SystemAPI.GetBuffer<VehicleSpringLengthCompressedBuffer>(parent.Value);

                springBuffer.RemoveAt(0);

                springBuffer.Add(new VehicleSpringLengthCompressedBuffer {
                    Index = index.Value,
                    Value = springRestDistance.Value - springCompression.Value
                });
            }
        }
    }
}
