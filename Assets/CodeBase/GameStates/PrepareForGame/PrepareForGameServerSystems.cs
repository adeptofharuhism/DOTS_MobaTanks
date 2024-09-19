using Assets.CodeBase.Finances;
using Assets.CodeBase.Infrastructure.PrefabInjection;
using Assets.CodeBase.Player;
using Assets.CodeBase.Player.PlayerCount;
using Assets.CodeBase.Player.Respawn;
using Assets.CodeBase.Teams;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

namespace Assets.CodeBase.GameStates.PrepareForGame
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(PrepareForGameStateSystemGroup))]
    [UpdateBefore(typeof(ServerProcessGameEntrySystem))]
    public partial struct ServerGhostRelevancySetSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<PrepareForGameState>();
            state.RequireForUpdate<GhostRelevancy>();
        }

        public void OnUpdate(ref SystemState state) {
            RefRW<GhostRelevancy> ghostRelevancySingleton = SystemAPI.GetSingletonRW<GhostRelevancy>();

            ghostRelevancySingleton.ValueRW.GhostRelevancyMode = GhostRelevancyMode.SetIsIrrelevant;

            state.Enabled = false;
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(PrepareForGameStateSystemGroup))]
    [UpdateAfter(typeof(ServerGhostRelevancySetSystem))]
    [UpdateBefore(typeof(EnterReportInGameStateSystem))]
    public partial struct ServerProcessGameEntrySystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            EntityQueryBuilder newPlayerDataRequestQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<SetNewPlayerDataRpc, ReceiveRpcCommandRequest>();
            state.RequireForUpdate(state.GetEntityQuery(newPlayerDataRequestQuery));

            state.RequireForUpdate<PrepareForGameState>();
            state.RequireForUpdate<ConnectedPlayerCount>();
            state.RequireForUpdate<GamePrefabs>();
            state.RequireForUpdate<BasicMoneyAmount>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            Entity playerPrefab = SystemAPI.GetSingleton<GamePrefabs>().Player;

            Entity vehiclePrefab = SystemAPI.GetSingleton<GamePrefabs>().Vehicle;
            RefRW<ConnectedPlayerCount> playerCount = SystemAPI.GetSingletonRW<ConnectedPlayerCount>();

            int basicMoneyAmount = SystemAPI.GetSingleton<BasicMoneyAmount>().Value;
            Entity financesPrefab = SystemAPI.GetSingleton<GhostFinancesPrefab>().Value;

            foreach (var (newPlayerData, requestSource, requestEntity)
                in SystemAPI.Query<SetNewPlayerDataRpc, ReceiveRpcCommandRequest>()
                .WithEntityAccess()) {

                ecb.DestroyEntity(requestEntity);
                ecb.AddComponent<NetworkStreamInGame>(requestSource.SourceConnection);

                int clientId = SystemAPI.GetComponent<NetworkId>(requestSource.SourceConnection).Value;
#if UNITY_EDITOR
                Debug.Log($"Connected {newPlayerData.PlayerName} with Client Id: {clientId}.");
#endif

                Entity playerEntity = ecb.Instantiate(playerPrefab);

                TeamType newPlayerTeam = GetNewPlayerTeam(playerCount.ValueRW.Value);

                ecb.SetComponent(playerEntity, new GhostOwner { NetworkId =  clientId });

                ecb.SetComponent(playerEntity, new VehicleRespawnParameters {
                    ClientId = clientId,
                    Team = newPlayerTeam,
                    VehiclePrefab = vehiclePrefab,
                    PlayerName = newPlayerData.PlayerName,
                    SpawnPosition = GetSpawnPosition(newPlayerTeam, playerCount.ValueRO.Value)
                });
                ecb.AddComponent<RespawnedEntity>(playerEntity);

                ecb.SetComponent(playerEntity, new MoneyAmount { Value = basicMoneyAmount });

                ecb.AddComponent(requestSource.SourceConnection, new PlayerEntity { Value = playerEntity });
                ecb.AppendToBuffer(requestSource.SourceConnection, new LinkedEntityGroup { Value = playerEntity });

                playerCount.ValueRW.Value++;
            }

            ecb.Playback(state.EntityManager);
        }

        private TeamType GetNewPlayerTeam(int playerCount) =>
            playerCount % 2 == 0 ? TeamType.Blue : TeamType.Orange;

        private float3 GetSpawnPosition(TeamType team, int playerCount) =>
            new float3((260 + 5 * (playerCount / 2)) * GetTeamSideMultiplier(team), 5, 50);

        private int GetTeamSideMultiplier(TeamType team) =>
            team == TeamType.Blue ? -1 : 1;
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(PrepareForGameStateSystemGroup))]
    [UpdateAfter(typeof(ServerProcessGameEntrySystem))]
    public partial struct EnterReportInGameStateSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<PrepareForGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (readyPlayers, connectedPlayers, minReadyPlayers)
                in SystemAPI.Query<ReadyPlayersCount, ConnectedPlayerCount, MinReadyPlayersToStartGame>()) {

                if (minReadyPlayers.Value > readyPlayers.Value)
                    return;

                if (readyPlayers.Value != connectedPlayers.Value)
                    return;
            }

            Entity stateEntity = SystemAPI.GetSingletonEntity<PrepareForGameState>();

            state.EntityManager.RemoveComponent<PrepareForGameState>(stateEntity);
            state.EntityManager.AddComponent<ReportInGameState>(stateEntity);
        }
    }
}
