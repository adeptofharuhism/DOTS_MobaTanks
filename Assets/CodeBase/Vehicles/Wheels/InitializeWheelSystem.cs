using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

namespace Assets.CodeBase.Vehicles.Wheels
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    [UpdateAfter(typeof(InitializeVehicleSystem))]
    public partial struct InitializeWheelSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NewWheelTag>();
            state.RequireForUpdate<NetworkTime>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            NetworkTick currentTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;

            foreach (var (newWheel, wheel)
                in SystemAPI.Query<NewWheelTag>()
                .WithEntityAccess()) {

                ecb.AddComponent(wheel, new WheelParent { Value = GetHighestParentOfEntity(ref state, wheel) });
                ecb.AddComponent(wheel, new WheelLatestForceApplyTick { Value = currentTick });

                if (state.WorldUnmanaged.IsClient())
                    ecb.AddComponent(wheel, new WheelSpringLengthCompressed { Value = 0 });

                ecb.RemoveComponent<NewWheelTag>(wheel);
                ecb.AddComponent<WheelInitializedTag>(wheel);
            }

            ecb.Playback(state.EntityManager);
        }

        private Entity GetHighestParentOfEntity(ref SystemState state, Entity entity) {
            bool hasParent = true;
            Entity parent = entity;
            while (hasParent) {
                if (SystemAPI.HasComponent<Parent>(parent)) {
                    parent = SystemAPI.GetComponent<Parent>(parent).Value;
                } else
                    hasParent = false;
            }

            return parent;
        }
    }
}
