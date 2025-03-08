using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Assets.CodeBase.Vehicles.Wheels
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(WheelPhysicsSystemGroup))]
    [UpdateBefore(typeof(WheelLookForGroundContactSystem))]
    public partial struct WheelCalculateLinearVelocitySystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (forceCastPoint, parent, wheel)
                in SystemAPI.Query<WheelForceCastPoint, WheelParent>()
                .WithAll<WheelInitializedTag>()
                .WithEntityAccess()) {

                RefRO<PhysicsVelocity> parentVelocity = SystemAPI.GetComponentRO<PhysicsVelocity>(parent.Value);
                RefRO<PhysicsMass> parentMass = SystemAPI.GetComponentRO<PhysicsMass>(parent.Value);
                RefRO<LocalToWorld> parentTransform = SystemAPI.GetComponentRO<LocalToWorld>(parent.Value);
                RefRO<LocalToWorld> forceCastTransform = SystemAPI.GetComponentRO<LocalToWorld>(forceCastPoint.Value);

                float3 linearVelocity =
                    Unity.Physics.Extensions.PhysicsComponentExtensions.GetLinearVelocity(
                        parentVelocity.ValueRO,
                        parentMass.ValueRO,
                        parentTransform.ValueRO.Position,
                        parentTransform.ValueRO.Rotation,
                        forceCastTransform.ValueRO.Position);

                ecb.SetComponent(wheel, new WheelLinearVelocity { Value = linearVelocity });
                ecb.SetComponent(wheel, new WheelAxisProjectedVelocity {
                    Value = new float3(
                        CalculateVelocityOnAxis(linearVelocity, forceCastTransform.ValueRO.Right),
                        CalculateVelocityOnAxis(linearVelocity, forceCastTransform.ValueRO.Up),
                        CalculateVelocityOnAxis(linearVelocity, forceCastTransform.ValueRO.Forward))
                });
            }
        }

        private float CalculateVelocityOnAxis(float3 linearVelocity, float3 axis) =>
            math.dot(linearVelocity, axis);
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(WheelPhysicsSystemGroup))]
    [UpdateAfter(typeof(WheelCalculateLinearVelocitySystem))]
    public partial struct WheelLookForGroundContactSystem : ISystem
    {
        private CollisionFilter _collisionFilter;

        public void OnCreate(ref SystemState state) {
            _collisionFilter = new CollisionFilter {
                BelongsTo = 1 << 2,
                CollidesWith = 1 << 0 | 1 << 1 | 1 << 3
            };

            state.RequireForUpdate<PhysicsWorldSingleton>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            CollisionWorld collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
            EntityCommandBuffer ecb =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (forceCastPoint, springRestDistance, wheel)
                in SystemAPI.Query<WheelForceCastPoint, WheelSpringRestDistance>()
                .WithAll<WheelInitializedTag>()
                .WithEntityAccess()) {

                RefRO<LocalToWorld> forceCastTransform = SystemAPI.GetComponentRO<LocalToWorld>(forceCastPoint.Value);

                RaycastInput raycastInput = new RaycastInput {
                    Start = forceCastTransform.ValueRO.Position,
                    End = forceCastTransform.ValueRO.Position -
                        math.normalize(forceCastTransform.ValueRO.Up) *
                        springRestDistance.Value,
                    Filter = _collisionFilter
                };

                bool hasHit = collisionWorld.CastRay(raycastInput, out RaycastHit closestHit);
                if (hasHit)
                    ecb.AddComponent<WheelHasGroundContactTag>(wheel);
                else
                    ecb.RemoveComponent<WheelHasGroundContactTag>(wheel);

                float compressionCoefficient = hasHit ? closestHit.Fraction : 1;
                ecb.SetComponent(wheel, new WheelSpringCompression {
                    Value = CalculateSpringCompression(springRestDistance.Value, compressionCoefficient)
                });
            }
        }

        private float CalculateSpringCompression(float springRestDistance, float compressionCoefficient) =>
            springRestDistance * (1 - compressionCoefficient);
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(WheelPhysicsSystemGroup))]
    [UpdateAfter(typeof(WheelLookForGroundContactSystem))]
    public partial struct WheelAccelerationEnableSystem : ISystem
    {
        private const float Epsilon = 1e-06f;

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (accelerationInput, wheel)
                in SystemAPI.Query<WheelAccelerationInput>()
                .WithAll<WheelInitializedTag, WheelHasGroundContactTag>()
                .WithEntityAccess()) {

                if (accelerationInput.Value > Epsilon || accelerationInput.Value < -Epsilon) {
                    ecb.RemoveComponent<WheelBrakingTag>(wheel);
                    ecb.AddComponent<WheelAcceleratingTag>(wheel);
                } else {
                    ecb.RemoveComponent<WheelAcceleratingTag>(wheel);
                    ecb.AddComponent<WheelBrakingTag>(wheel);
                }
            }
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(WheelPhysicsSystemGroup))]
    [UpdateAfter(typeof(WheelAccelerationEnableSystem))]
    public partial struct WheelAccelerationForceCalculationSystem : ISystem
    {
        private const float Epsilon = 1e-06f;
        private const float HyperbolicOffsetY = 1.02f;
        private const float HyperbolaAngleMultiplier = 0.03f;
        private const float HyperbolicOffsetX = -1.03f;
        private const float HardBrakingVelocityValue = 0;

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (accelerationParameters, accelerationInput, axisProjectedVelocity, forceCastPosition, wheel)
                in SystemAPI.Query<WheelAccelerationParameters, WheelAccelerationInput, WheelAxisProjectedVelocity, WheelForceCastPoint>()
                .WithAll<WheelInitializedTag, WheelHasGroundContactTag, WheelAcceleratingTag>()
                .WithEntityAccess()) {

                RefRO<LocalToWorld> forceCastTransform = SystemAPI.GetComponentRO<LocalToWorld>(forceCastPosition.Value);

                float3 forceCastForward = forceCastTransform.ValueRO.Forward;

                float zForceValue = CalculateZForce(
                    accelerationInput.Value,
                    axisProjectedVelocity.Value.z,
                    accelerationParameters.MaxVelocity,
                    accelerationParameters.MaxVelocityBackwards,
                    accelerationParameters.HardBrakingForceMultiplier,
                    accelerationParameters.EngineForceMultiplier);

                float3 zForceVector = forceCastForward * zForceValue;

                ecb.SetComponent(wheel, new WheelAxisForceAcceleration { Value = zForceVector });
            }
        }

        private float CalculateZForce(
            float accelerationInput,
            float velocityZ,
            float maxVelocity,
            float maxVelocityBackwards,
            float hardBrakingForceMultiplier,
            float engineForceMultiplier) =>

            (accelerationInput > Epsilon)
                ? velocityZ > maxVelocity
                    ? 0
                    : CalculateAccelerationForce(
                            accelerationInput,
                            velocityZ,
                            maxVelocity,
                            hardBrakingForceMultiplier,
                            engineForceMultiplier)
                : velocityZ < maxVelocityBackwards
                    ? 0
                    : CalculateAccelerationForce(
                            accelerationInput,
                            velocityZ,
                            maxVelocityBackwards,
                            hardBrakingForceMultiplier,
                            engineForceMultiplier);

        private float CalculateAccelerationForce(
            float accelerationInput,
            float velocityZ,
            float maxVelocity,
            float hardBrakingForceMultiplier,
            float engineForceMultiplier) =>

            (velocityZ * accelerationInput > 0
                ? CalculateEngineForce(accelerationInput, velocityZ, maxVelocity)
                : CalculateEngineForce(accelerationInput, HardBrakingVelocityValue, maxVelocity)
                    * hardBrakingForceMultiplier
            ) * engineForceMultiplier;

        private float CalculateEngineForce(float accelerationInput, float velocity, float maxSpeed) =>
            accelerationInput * HyperbolicCurve(velocity / maxSpeed);

        private float HyperbolicCurve(float x) =>
            HyperbolicOffsetY + (HyperbolaAngleMultiplier / (x + HyperbolicOffsetX));
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(WheelPhysicsSystemGroup))]
    [UpdateAfter(typeof(WheelAccelerationForceCalculationSystem))]
    public partial struct WheelSteeringForceCalculationSystem : ISystem
    {
        private const float Epsilon = 1E-06f;

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (steeringParameters, axisProjectedVelocity, forceCastPoint, wheel)
                in SystemAPI.Query<WheelSteeringParameters, WheelAxisProjectedVelocity, WheelForceCastPoint>()
                .WithAll<WheelInitializedTag, WheelHasGroundContactTag>()
                .WithEntityAccess()) {

                RefRO<LocalToWorld> forceCastTransform = SystemAPI.GetComponentRO<LocalToWorld>(forceCastPoint.Value);

                ecb.SetComponent(wheel, new WheelAxisForceSteering {
                    Value = CalculateXVector(
                        forceCastTransform.ValueRO.Right,
                        steeringParameters.MaximalSteering,
                        steeringParameters.MinimalSteering,
                        axisProjectedVelocity.Value.x,
                        axisProjectedVelocity.Value.z)
                });
            }
        }

        private float3 CalculateXVector(float3 forceCastRight, float maxSteering, float minSteering, float velocityX, float velocityZ) =>
            -forceCastRight * CalculateXForce(maxSteering, minSteering, velocityX, velocityZ);

        private float CalculateXForce(float maxSteering, float minSteering, float velocityX, float velocityZ) =>
            CalculateTractionCoefficient(maxSteering, minSteering, velocityX, velocityZ) * velocityX;

        private float CalculateTractionCoefficient(float maxSteering, float minSteering, float velocityX, float velocityZ) =>
            math.lerp(maxSteering, minSteering, CalculateXVelocityToHorizontalVelocityRatio(velocityX, velocityZ));

        private float CalculateXVelocityToHorizontalVelocityRatio(float velocityX, float velocityZ) =>
            math.square(velocityX) / (Epsilon + math.square(velocityX) + math.square(velocityZ));
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(WheelPhysicsSystemGroup))]
    [UpdateAfter(typeof(WheelSteeringForceCalculationSystem))]
    public partial struct WheelBrakingForceCalculationSystem : ISystem
    {
        private const float Epsilon = .2f;
        private const float BrakingStrength = 5f;

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (axisProjectedVelocity, forceCastPoint, wheel)
                in SystemAPI.Query<WheelAxisProjectedVelocity, WheelForceCastPoint>()
                .WithAll<WheelBrakingTag, WheelInitializedTag, WheelHasGroundContactTag>()
                .WithEntityAccess()) {

                RefRO<LocalToWorld> forceCastTransform = SystemAPI.GetComponentRO<LocalToWorld>(forceCastPoint.Value);

                float3 forceCastForward = forceCastTransform.ValueRO.Forward;

                float3 zForceVector =
                    forceCastForward * CalculateZForce(ref state, axisProjectedVelocity.Value.z);

                ecb.SetComponent(wheel, new WheelAxisForceAcceleration { Value = zForceVector });
            }
        }

        private float CalculateZForce(ref SystemState state, float velocityZ) =>
            CalculateBraking(velocityZ) * SystemAPI.Time.DeltaTime;

        private float CalculateBraking(float velocity) {
            if (velocity > Epsilon)
                return -BrakingStrength;

            if (velocity < -Epsilon)
                return BrakingStrength;

            return -velocity;
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(WheelPhysicsSystemGroup))]
    [UpdateAfter(typeof(WheelBrakingForceCalculationSystem))]
    public partial struct WheelSpringForceCalculationSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (springCompression, axisProjectedVelocity, springStrength, forceCastPosition, wheel)
                in SystemAPI.Query<WheelSpringCompression, WheelAxisProjectedVelocity, WheelSpringStrength, WheelForceCastPoint>()
                  .WithAll<WheelInitializedTag, WheelHasGroundContactTag>()
                  .WithEntityAccess()) {

                RefRO<LocalToWorld> forceCastTransform = SystemAPI.GetComponentRO<LocalToWorld>(forceCastPosition.Value);

                float3 forceCastUp = forceCastTransform.ValueRO.Up;

                float3 yForceVector =
                    forceCastUp * CalculateYForce(
                        ref state,
                        springStrength.Strength,
                        springCompression.Value,
                        springStrength.Damper,
                        axisProjectedVelocity.Value.y);

                ecb.SetComponent(wheel, new WheelAxisForceSpring { Value = yForceVector });
            }
        }

        private float CalculateYForce(
            ref SystemState state,
            float springStrength,
            float springCompression,
            float springDamper,
            float velocityY) =>

            (springStrength * springCompression - springDamper * velocityY) * SystemAPI.Time.DeltaTime;
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(WheelPhysicsSystemGroup))]
    [UpdateAfter(typeof(WheelSpringForceCalculationSystem))]
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



    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(WheelPhysicsSystemGroup))]
    public partial struct ClientWheelDetermineCompressedLengthSystem : ISystem
    {
        private CollisionFilter _collisionFilter;

        public void OnCreate(ref SystemState state) {
            _collisionFilter = new CollisionFilter {
                BelongsTo = 1 << 2,
                CollidesWith = 1 << 0 | 1 << 1 | 1 << 3
            };

            state.RequireForUpdate<PhysicsWorldSingleton>();
            state.RequireForUpdate<WheelModelUpdateCountdown>();
        }

        public void OnUpdate(ref SystemState state) {
            CollisionWorld collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
            RefRW<WheelModelUpdateCountdown> updateCountdown = SystemAPI.GetSingletonRW<WheelModelUpdateCountdown>();

            updateCountdown.ValueRW.TimeLeftForNextUpdate -= SystemAPI.Time.DeltaTime;
            if (updateCountdown.ValueRO.TimeLeftForNextUpdate > 0)
                return;

            updateCountdown.ValueRW.TimeLeftForNextUpdate = updateCountdown.ValueRO.AwaitTime;

            foreach (var (forceCastPoint, springRestDistance, compressedLength)
                in SystemAPI.Query<WheelForceCastPoint, WheelSpringRestDistance, RefRW<WheelCompressedSpringLength>>()
                .WithAll<WheelInitializedTag>()) {

                RefRO<LocalToWorld> forceCastTransform = SystemAPI.GetComponentRO<LocalToWorld>(forceCastPoint.Value);

                RaycastInput raycastInput = new RaycastInput {
                    Start = forceCastTransform.ValueRO.Position,
                    End = forceCastTransform.ValueRO.Position -
                        math.normalize(forceCastTransform.ValueRO.Up) *
                        springRestDistance.Value,
                    Filter = _collisionFilter
                };

                bool hasHit = collisionWorld.CastRay(raycastInput, out RaycastHit closestHit);

                float compressionCoefficient = hasHit ? closestHit.Fraction : 1;

                compressedLength.ValueRW.Value = springRestDistance.Value * compressionCoefficient;
            }
        }
    }
}
