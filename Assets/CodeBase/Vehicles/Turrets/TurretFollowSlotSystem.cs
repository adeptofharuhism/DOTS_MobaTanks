using Unity.Entities;
using Unity.Transforms;

namespace Assets.CodeBase.Vehicles.Turrets
{
    [UpdateInGroup(typeof(TransformSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct TurretSetInSlotSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            foreach (var (model, slot, rotationAngle)
                in SystemAPI.Query<TurretModel, TurretSlot, TurretRotationAngle>()
                .WithAll<TurretModelInitialized>()) {

                RefRO<LocalToWorld> slotTransform = SystemAPI.GetComponentRO<LocalToWorld>(slot.Value);
                RefRW<LocalTransform> modelTransform = SystemAPI.GetComponentRW<LocalTransform>(model.Value);

                modelTransform.ValueRW.Position = slotTransform.ValueRO.Position;
                modelTransform.ValueRW.Rotation = slotTransform.ValueRO.Rotation;
                modelTransform.ValueRW.Rotation = modelTransform.ValueRW.RotateY(rotationAngle.Value).Rotation;
            }
        }
    }
}
