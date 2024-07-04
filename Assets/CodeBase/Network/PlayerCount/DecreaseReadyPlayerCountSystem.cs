using Unity.Entities;

namespace Assets.CodeBase.Network.PlayerCount
{
    [UpdateInGroup(typeof(NetworkProcessSystemGroup))]
    [UpdateBefore(typeof(DecreaseConnectedPlayerCountSystem))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct DecreaseReadyPlayerCountSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<ReadyPlayersCount>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            RefRW<ReadyPlayersCount> readyCount = SystemAPI.GetSingletonRW<ReadyPlayersCount>();

            foreach (var (tag, entity)
                in SystemAPI.Query<DecreaseReadyPlayerCountOnCleanUpTag>()
                .WithNone<PlayerReady>()
                .WithEntityAccess()) {

                readyCount.ValueRW.Value--;

                ecb.RemoveComponent<DecreaseReadyPlayerCountOnCleanUpTag>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
