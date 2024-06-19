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
        private const float CameraAdvanceMultiplier = 50f;

        private float3 _previousFramePosition;

        protected override void OnCreate() {
            _previousFramePosition = float3.zero;

            RequireForUpdate<OwnerVehicleTag>();
        }

        protected override void OnUpdate() {
            Entity ownedEntity = SystemAPI.GetSingletonEntity<OwnerVehicleTag>();

            RefRO<LocalToWorld> ownedEntityTransform = SystemAPI.GetComponentRO<LocalToWorld>(ownedEntity);

            float3 currentFramePosition = ownedEntityTransform.ValueRO.Position;

            float3 positionDifference = currentFramePosition - _previousFramePosition;
            positionDifference.y = 0;
            positionDifference *= CameraAdvanceMultiplier;

            CameraSingleton.Instance.TargetPosition = ownedEntityTransform.ValueRO.Position + positionDifference;

            _previousFramePosition = currentFramePosition;
        }
    }
}