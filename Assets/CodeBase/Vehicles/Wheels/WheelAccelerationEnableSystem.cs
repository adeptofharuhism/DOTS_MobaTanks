using Unity.Burst;
using Unity.Entities;
using Unity.Physics.Systems;

namespace Assets.CodeBase.Vehicles.Wheels
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
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
}