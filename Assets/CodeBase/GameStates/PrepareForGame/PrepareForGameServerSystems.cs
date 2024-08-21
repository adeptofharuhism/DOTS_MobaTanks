using Assets.CodeBase.Combat.Teams;
using Assets.CodeBase.Finances;
using Assets.CodeBase.Infrastructure.PlayerCount;
using Assets.CodeBase.Infrastructure.PrefabInjection;
using Assets.CodeBase.Infrastructure.Respawn;
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
        private const int PlayerRespawnCooldown = 10;

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

                Entity playerEntity = requestSource.SourceConnection;
                AddRespawnComponents(
                    ref ecb,
                    playerEntity,
                    playerCount.ValueRW.Value,
                    clientId,
                    newPlayerData.PlayerName,
                    vehiclePrefab);
                AddPlayerCountComponents(ref ecb, playerEntity);
                AddFinancesComponents(ref ecb, playerEntity, financesPrefab, basicMoneyAmount, clientId);

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

        private void AddFinancesComponents(
            ref EntityCommandBuffer ecb,
            Entity entity,
            Entity financesPrefab,
            int basicMoneyAmount,
            int clientId) {

            Entity financesEntity = ecb.Instantiate(financesPrefab);
            ecb.SetComponent(financesEntity, new MoneyAmount { Value = basicMoneyAmount });
            ecb.SetComponent(financesEntity, new GhostFinancesConnectionId { Value = clientId });

            ecb.AddComponent(entity, new FinancesEntity { Value = financesEntity });
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
