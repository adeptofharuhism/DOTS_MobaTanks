using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Assets.CodeBase.Vehicles.Turrets
{
    [UpdateInGroup(typeof(TransformSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct TurretSetRotationSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (model, rotationAngle)
                in SystemAPI.Query<TurretModel, TurretRotationAngle>()
                .WithAll<TurretModelInitialized>()) {

                RefRW<LocalTransform> modelTransform = SystemAPI.GetComponentRW<LocalTransform>(model.Value);

                modelTransform.ValueRW.Rotation = quaternion.EulerXYZ(0, rotationAngle.Value, 0);
            }
        }
    }
}
