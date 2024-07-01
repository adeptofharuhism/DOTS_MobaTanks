using Assets.CodeBase.Vehicles;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Assets.CodeBase.Camera
{
    [UpdateInGroup(typeof(TransformSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial class CameraFollowOwnedVehicleSystem : SystemBase
    {
        private const float CameraAdvanceMultiplier = 20f;

        protected override void OnCreate() {
            RequireForUpdate<OwnerVehicleTag>();
        }

        protected override void OnUpdate() {
            Entity ownedEntity = SystemAPI.GetSingletonEntity<OwnerVehicleTag>();

            RefRO<LocalToWorld> ownedEntityTransform = SystemAPI.GetComponentRO<LocalToWorld>(ownedEntity);

            float3 forward = ownedEntityTransform.ValueRO.Forward;
            float3 cameraLookOffset = forward * CameraAdvanceMultiplier;
            cameraLookOffset.y = 0;

            CameraSingleton.Instance.TargetPosition = ownedEntityTransform.ValueRO.Position + cameraLookOffset;
        }
    }
}