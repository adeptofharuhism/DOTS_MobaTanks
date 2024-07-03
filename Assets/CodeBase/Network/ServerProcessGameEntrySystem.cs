using Assets.CodeBase.Combat.Teams;
using Assets.CodeBase.Network.PlayerCount;
using Assets.CodeBase.Infrastructure.PrefabInjection;
using Assets.CodeBase.Infrastructure.Respawn;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

namespace Assets.CodeBase.Network
{
    [UpdateInGroup(typeof(NetworkProcessSystemGroup), OrderLast = true)]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct ServerProcessGameEntrySystem : ISystem
    {
        private const int PlayerRespawnCooldown = 10;

        public void OnCreate(ref SystemState state) {
            EntityQueryBuilder newPlayerDataRequestQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<SetNewPlayerDataRequest, ReceiveRpcCommandRequest>();
            state.RequireForUpdate(state.GetEntityQuery(newPlayerDataRequestQuery));

            state.RequireForUpdate<ConnectedPlayerCount>();
            state.RequireForUpdate<GamePrefabs>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            Entity vehiclePrefab = SystemAPI.GetSingleton<GamePrefabs>().Vehicle;
            RefRW<ConnectedPlayerCount> playerCount = SystemAPI.GetSingletonRW<ConnectedPlayerCount>();

            foreach (var (newPlayerData, requestSource, requestEntity)
                in SystemAPI.Query<SetNewPlayerDataRequest, ReceiveRpcCommandRequest>()
                .WithEntityAccess()) {

                ecb.DestroyEntity(requestEntity);
                ecb.AddComponent<NetworkStreamInGame>(requestSource.SourceConnection);

                int clientId = SystemAPI.GetComponent<NetworkId>(requestSource.SourceConnection).Value;
                Debug.Log($"Connected {newPlayerData.PlayerName} with Client Id: {clientId}.");

                Entity playerEntity = requestSource.SourceConnection;
                AddRespawnComponents(
                    ref ecb,
                    playerEntity,
                    playerCount.ValueRW.Value,
                    clientId,
                    newPlayerData.PlayerName,
                    vehiclePrefab);
                AddPlayerCountComponents(ref ecb, playerEntity);

                playerCount.ValueRW.Value++;
            }

            ecb.Playback(state.EntityManager);
        }

        private void AddRespawnComponents(
            ref EntityCommandBuffer ecb,
            Entity entity,
            int playerCount,
            int clientId,
            FixedString64Bytes playerName,
            Entity vehiclePrefab) {

            TeamType newPlayerTeam = GetNewPlayerTeam(playerCount);

            ecb.AddComponent(entity, new VehicleRespawnParameters {
                ClientId = clientId,
                Team = newPlayerTeam,
                VehiclePrefab = vehiclePrefab,
                PlayerName = playerName,
                SpawnPosition = GetSpawnPosition(newPlayerTeam, playerCount),
            });
            ecb.AddComponent(entity, new RespawnCooldown { Value = PlayerRespawnCooldown });
            ecb.AddComponent<TimeToRespawn>(entity);
            ecb.AddComponent<ShouldRespawnTag>(entity);
            ecb.AddComponent<RespawnedEntity>(entity);
        }

        private void AddPlayerCountComponents(ref EntityCommandBuffer ecb, Entity entity) {
            ecb.AddComponent<CountAsPlayerTag>(entity);
            ecb.AddComponent<DecreaseConnectedPlayerCountOnCleanUpTag>(entity);
        }

        private TeamType GetNewPlayerTeam(int playerCount) =>
            (playerCount % 2 == 0) ? TeamType.Blue : TeamType.Orange;

        private float3 GetSpawnPosition(TeamType team, int playerCount) =>
            new float3((260 + 5 * (playerCount / 2)) * GetTeamSideMultiplier(team), 5, 50);

        private int GetTeamSideMultiplier(TeamType team) =>
            (team == TeamType.Blue) ? -1 : 1;
    }
}
