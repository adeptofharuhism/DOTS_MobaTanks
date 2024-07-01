using Assets.CodeBase.Combat.Health;
using Assets.CodeBase.Combat.Teams;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

namespace Assets.CodeBase.Infrastructure.Respawn
{
    [UpdateAfter(typeof(RespawnVehicleCooldownSystem))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct RespawnVehicleSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (respawnParameters, respawnParametersEntity)
                in SystemAPI.Query<VehicleRespawnParameters>()
                .WithAll<ShouldRespawnTag>()
                .WithEntityAccess()) {

                Entity newVehicle = ecb.Instantiate(respawnParameters.VehiclePrefab);
                ecb.SetName(newVehicle, respawnParameters.PlayerName);

                ecb.SetComponent(newVehicle, new PlayerName { Value = respawnParameters.PlayerName });

                ecb.SetComponent(newVehicle, new GhostOwner { NetworkId = respawnParameters.ClientId });

                ecb.SetComponent(newVehicle, new UnitTeam { Value = respawnParameters.Team });

                LocalTransform vehicleTransform = LocalTransform.FromPosition(respawnParameters.SpawnPosition);
                ecb.SetComponent(newVehicle, vehicleTransform);

                ecb.SetComponent(respawnParametersEntity, new RespawnedEntity { Value = newVehicle });

                ecb.RemoveComponent<ShouldRespawnTag>(respawnParametersEntity);
                ecb.AddComponent<ChecksRespawnedEntityPresenceTag>(respawnParametersEntity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
