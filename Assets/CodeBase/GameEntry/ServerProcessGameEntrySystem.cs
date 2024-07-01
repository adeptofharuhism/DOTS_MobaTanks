using Assets.CodeBase.Combat.Teams;
using Assets.CodeBase.Infrastructure.PrefabInjection;
using Assets.CodeBase.Infrastructure.Respawn;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

namespace Assets.CodeBase.GameEntry
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct ServerProcessGameEntrySystem : ISystem
    {
        private int _playersInGame;

        public void OnCreate(ref SystemState state) {
            _playersInGame = 0;

            EntityQueryBuilder newPlayerDataRequestQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<SetNewPlayerDataRequest, ReceiveRpcCommandRequest>();
            state.RequireForUpdate(state.GetEntityQuery(newPlayerDataRequestQuery));

            state.RequireForUpdate<GamePrefabs>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            Entity vehiclePrefab = SystemAPI.GetSingleton<GamePrefabs>().Vehicle;

            foreach (var (newPlayerData, requestSource, requestEntity)
                in SystemAPI.Query<SetNewPlayerDataRequest, ReceiveRpcCommandRequest>()
                .WithEntityAccess()) {

                ecb.DestroyEntity(requestEntity);
                ecb.AddComponent<NetworkStreamInGame>(requestSource.SourceConnection);

                int clientId = SystemAPI.GetComponent<NetworkId>(requestSource.SourceConnection).Value;
                Debug.Log($"Connected {newPlayerData.PlayerName} with Client Id: {clientId}");

                TeamType newPlayerTeam = GetNewPlayerTeam();

                Entity persistentPlayerEntity = ecb.CreateEntity();
                ecb.SetName(persistentPlayerEntity, $"{newPlayerData.PlayerName}RespawnParameters");
                ecb.AddComponent(persistentPlayerEntity, new VehicleRespawnParameters {
                    ClientId = clientId,
                    Team = newPlayerTeam,
                    VehiclePrefab = vehiclePrefab,
                    PlayerName = newPlayerData.PlayerName,
                    SpawnPosition = GetSpawnPosition(newPlayerTeam),
                });
                ecb.AddComponent(persistentPlayerEntity, new RespawnCooldown { Value = 10 });
                ecb.AddComponent<TimeToRespawn>(persistentPlayerEntity);
                ecb.AddComponent<ShouldRespawnTag>(persistentPlayerEntity);
                ecb.AddComponent<RespawnedEntity>(persistentPlayerEntity);

                ecb.AppendToBuffer(requestSource.SourceConnection, new LinkedEntityGroup { Value = persistentPlayerEntity });
            }

            ecb.Playback(state.EntityManager);
        }

        private TeamType GetNewPlayerTeam() =>
            (_playersInGame++ % 2 == 0) ? TeamType.Blue : TeamType.Orange;

        private float3 GetSpawnPosition(TeamType team) =>
            new float3((260 + 5 * (_playersInGame / 2)) * GetTeamSideMultiplier(team), 5, 50);

        private int GetTeamSideMultiplier(TeamType team) =>
            (team == TeamType.Blue) ? -1 : 1;
    }
}
