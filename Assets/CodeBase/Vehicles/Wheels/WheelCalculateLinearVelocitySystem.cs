using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace Assets.CodeBase.Vehicles.Wheels
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(PhysicsSimulationGroup))]
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

        [BurstCompile]
        private float CalculateVelocityOnAxis(float3 linearVelocity, float3 axis) =>
            math.dot(linearVelocity, axis);
    }
}
