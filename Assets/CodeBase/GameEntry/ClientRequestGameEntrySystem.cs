using Assets.CodeBase.Infrastructure.PrefabInjection;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.GameEntry
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct ClientRequestGameEntrySystem : ISystem
    {
        public EntityQuery _pendingNetworkQuery;

        public void OnCreate(ref SystemState state) {
            EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<NetworkId>()
                .WithNone<NetworkStreamInGame>();
            _pendingNetworkQuery = state.GetEntityQuery(builder);

            state.RequireForUpdate(_pendingNetworkQuery);
            state.RequireForUpdate<ConnectionRequestData>();
            state.RequireForUpdate<SubScenesLoadPassedTag>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
            NativeArray<Entity> pendingNetworkIds = _pendingNetworkQuery.ToEntityArray(Allocator.Temp);
            FixedString64Bytes playerName = SystemAPI.GetSingleton<ConnectionRequestData>().PlayerName;

            foreach (Entity pendingNetworkId in pendingNetworkIds) {
                ecb.AddComponent<NetworkStreamInGame>(pendingNetworkId);

                Entity playerDataEntity = ecb.CreateEntity();

                ecb.AddComponent(playerDataEntity, new SetNewPlayerDataRequest { PlayerName = playerName });
                ecb.AddComponent(playerDataEntity, new SendRpcCommandRequest { TargetConnection = pendingNetworkId });
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
