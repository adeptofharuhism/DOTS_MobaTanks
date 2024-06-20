using Unity.Entities;
using Unity.NetCode;
using Unity.Collections;
using UnityEngine;
using Assets.CodeBase.Infrastructure.PrefabInjection;
using Unity.Mathematics;
using Unity.Transforms;
using Assets.CodeBase.Combat.Teams;

namespace Assets.CodeBase.GameEntry
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct ServerProcessGameEntrySystem : ISystem
    {
        private const string VehicleName = "Vehicle";

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

                Entity newVehicle = ecb.Instantiate(vehiclePrefab);
                ecb.SetName(newVehicle, newPlayerData.PlayerName);

                ecb.SetComponent(newVehicle, new GhostOwner { NetworkId = clientId });

                ecb.AppendToBuffer(requestSource.SourceConnection, new LinkedEntityGroup { Value = newVehicle });

                TeamType newPlayerTeam = GetNewPlayerTeam();
                ecb.SetComponent(newVehicle, new UnitTeam { Value = newPlayerTeam });

                int teamSideMultiplier;
                if (newPlayerTeam == TeamType.Blue) {
                    teamSideMultiplier = -1;
                } else {
                    teamSideMultiplier = 1;
                }

                float3 vehicleSpawnPosition = new float3((260 + 5 * clientId) * teamSideMultiplier, 5, 50);
                LocalTransform vehicleTransform = LocalTransform.FromPosition(vehicleSpawnPosition);
                ecb.SetComponent(newVehicle, vehicleTransform);
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
    }
}
