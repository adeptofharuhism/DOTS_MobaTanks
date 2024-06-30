using Unity.Entities;
using Unity.NetCode;
using Unity.Collections;
using UnityEngine;
using Assets.CodeBase.Infrastructure.PrefabInjection;
using Unity.Mathematics;
using Unity.Transforms;
using Assets.CodeBase.Combat.Teams;
using Assets.CodeBase.Combat.Health;
using Assets.CodeBase.Infrastructure.Respawn;
using Assets.CodeBase.Infrastructure.Destruction;

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

                Entity vehicleRespawnParameters = ecb.CreateEntity();
                ecb.SetName(vehicleRespawnParameters, $"{newPlayerData.PlayerName}RespawnParameters");
                ecb.AddComponent(vehicleRespawnParameters, new VehicleRespawnParameters {
                    ClientId = clientId,
                    Team = newPlayerTeam,
                    VehiclePrefab = vehiclePrefab,
                    PlayerName = newPlayerData.PlayerName,
                    SpawnPosition = GetSpawnPosition(newPlayerTeam),
                });
                ecb.AddComponent(vehicleRespawnParameters, new RespawnCooldown { Value = 10 });
                ecb.AddComponent<TimeToRespawn>(vehicleRespawnParameters);
                ecb.AddComponent<ShouldRespawnTag>(vehicleRespawnParameters);
                ecb.AddComponent<RespawnedEntity>(vehicleRespawnParameters);

                ecb.AppendToBuffer(requestSource.SourceConnection, new LinkedEntityGroup { Value = vehicleRespawnParameters });
            }

            ecb.Playback(state.EntityManager);
        }

        private TeamType GetNewPlayerTeam() {
            TeamType result;
            if (_playersInGame++ % 2 == 0)
                result = TeamType.Blue;
            else
                result = TeamType.Orange;

            return result;
        }

        private float3 GetSpawnPosition(TeamType team) =>
            new float3((260 + 5 * (_playersInGame / 2)) * GetTeamSideMultiplier(team), 5, 50);

        private int GetTeamSideMultiplier(TeamType team) {
            int teamSideMultiplier;
            if (team == TeamType.Blue)
                teamSideMultiplier = -1;
            else
                teamSideMultiplier = 1;

            return teamSideMultiplier;
        }
    }
}
