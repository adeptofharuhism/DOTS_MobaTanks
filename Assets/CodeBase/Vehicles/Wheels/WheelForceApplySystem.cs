using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace Assets.CodeBase.Vehicles.Wheels
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(WheelSpringForceCalculationSystem))]
    [UpdateAfter(typeof(WheelSteeringForceCalculationSystem))]
    [UpdateAfter(typeof(WheelBrakingForceCalculationSystem))]
    [UpdateAfter(typeof(WheelAccelerationForceCalculationSystem))]
    public partial struct WheelForceApplySystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            foreach (var (springForce, steeringForce, accelerationForce, forceCastPoint, parent, index)
                in SystemAPI.Query<WheelAxisForceSpring, WheelAxisForceSteering, WheelAxisForceAcceleration, WheelForceCastPoint, WheelParent, WheelIndex>()
                .WithAll<WheelInitializedTag, WheelHasGroundContactTag>()) {

                RefRO<LocalToWorld> forceCastTransform = SystemAPI.GetComponentRO<LocalToWorld>(forceCastPoint.Value);
                RefRO<LocalToWorld> parentTransform = SystemAPI.GetComponentRO<LocalToWorld>(parent.Value);
                RefRW<PhysicsVelocity> parentVelocity = SystemAPI.GetComponentRW<PhysicsVelocity>(parent.Value);
                RefRW<PhysicsMass> parentMass = SystemAPI.GetComponentRW<PhysicsMass>(parent.Value);

                float3 forceVector = springForce.Value;// + accelerationForce.Value;// + steeringForce.Value;

                //if (index.Value == 0) {
                //    UnityEngine.Debug.DrawLine(forceCastTransform.ValueRO.Position, forceCastTransform.ValueRO.Position + forceVector, UnityEngine.Color.magenta);
                //    UnityEngine.Debug.DrawLine(forceCastTransform.ValueRO.Position, forceCastTransform.ValueRO.Position + springForce.Value, UnityEngine.Color.green);
                //    //UnityEngine.Debug.DrawLine(forceCastTransform.ValueRO.Position, forceCastTransform.ValueRO.Position + steeringForce.Value, UnityEngine.Color.red);
                //    //UnityEngine.Debug.DrawLine(forceCastTransform.ValueRO.Position, forceCastTransform.ValueRO.Position + accelerationForce.Value, UnityEngine.Color.blue);
                //}

                Unity.Physics.Extensions.PhysicsComponentExtensions.ApplyImpulse(
                    ref parentVelocity.ValueRW,
                    parentMass.ValueRW,
                    parentTransform.ValueRO.Position,
                    parentTransform.ValueRO.Rotation,
                    forceVector,
                    forceCastTransform.ValueRO.Position);
            }
        }
    }
}
