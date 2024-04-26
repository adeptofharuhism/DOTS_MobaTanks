using Assets.CodeBase.Vehicles;
using Unity.Entities;
using Unity.Transforms;

namespace Assets.CodeBase.Camera
{
    [UpdateInGroup(typeof(TransformSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial class CameraFollowOwnedVehicleSystem : SystemBase
    {
        protected override void OnCreate() {
            RequireForUpdate<OwnerVehicleTag>();
        }

        protected override void OnUpdate() {
            Entity ownedEntity = SystemAPI.GetSingletonEntity<OwnerVehicleTag>();

            RefRO<LocalToWorld> ownedEntityTransform = SystemAPI.GetComponentRO<LocalToWorld>(ownedEntity);

            CameraSingleton.Instance.TargetPosition = ownedEntityTransform.ValueRO.Position;
        }
    }
}