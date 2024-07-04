using Unity.Entities;

namespace Assets.CodeBase.Network.GameStart
{
    [UpdateInGroup(typeof(NetworkProcessSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct DecreaseConnectedPlayerCountSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<ConnectedPlayerCount>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            RefRW<ConnectedPlayerCount> playerCount = SystemAPI.GetSingletonRW<ConnectedPlayerCount>();

            foreach (var (tag, entity)
                in SystemAPI.Query<DecreaseConnectedPlayerCountOnCleanUpTag>()
                .WithNone<CountAsPlayerTag>()
                .WithEntityAccess()) {

                playerCount.ValueRW.Value--;

                ecb.RemoveComponent<DecreaseConnectedPlayerCountOnCleanUpTag>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
