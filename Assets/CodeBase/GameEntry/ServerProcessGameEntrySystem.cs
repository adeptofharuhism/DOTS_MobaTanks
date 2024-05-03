using Unity.Entities;
using Unity.NetCode;
using Unity.Collections;
using UnityEngine;
using Assets.CodeBase.Infrastructure.PrefabInjection;
using Unity.Mathematics;
using Unity.Transforms;

namespace Assets.CodeBase.GameEntry
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct ServerProcessGameEntrySystem : ISystem
    {
        private const string VehicleName = "Vehicle";

        public void OnCreate(ref SystemState state) {
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
                ecb.SetName(newVehicle, VehicleName);

                float3 vehicleSpawnPosition = new float3(5 * clientId, 5, 0);
                LocalTransform vehicleTransform = LocalTransform.FromPosition(vehicleSpawnPosition);

                ecb.SetComponent(newVehicle, vehicleTransform);
                ecb.SetComponent(newVehicle, new GhostOwner { NetworkId = clientId });

                ecb.AppendToBuffer(requestSource.SourceConnection, new LinkedEntityGroup { Value = newVehicle });
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
