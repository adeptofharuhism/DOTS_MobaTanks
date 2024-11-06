using Assets.CodeBase.Vehicles;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Assets.CodeBase.Camera
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(CameraSystemGroup))]
    public partial class CameraFollowOwnedVehicleSystem : SystemBase
    {
        private const float CameraAdvanceMultiplier = 20f;

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