﻿using Unity.Burst;
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
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (springForce, steeringForce, accelerationForce, forceCastPoint, parent)
                in SystemAPI.Query<
                    WheelAxisForceSpring,
                    WheelAxisForceSteering,
                    WheelAxisForceAcceleration,
                    WheelForceCastPoint,
                    WheelParent>()
                .WithAll<WheelInitializedTag, WheelHasGroundContactTag>()) {

                RefRO<LocalToWorld> forceCastTransform = SystemAPI.GetComponentRO<LocalToWorld>(forceCastPoint.Value);
                RefRO<LocalToWorld> parentTransform = SystemAPI.GetComponentRO<LocalToWorld>(parent.Value);
                RefRW<PhysicsVelocity> parentVelocity = SystemAPI.GetComponentRW<PhysicsVelocity>(parent.Value);
                RefRW<PhysicsMass> parentMass = SystemAPI.GetComponentRW<PhysicsMass>(parent.Value);

                float3 forceVector = springForce.Value + accelerationForce.Value + steeringForce.Value;

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
