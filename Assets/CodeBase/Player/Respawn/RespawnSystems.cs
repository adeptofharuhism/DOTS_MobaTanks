using Assets.CodeBase.Combat.Health;
using Assets.CodeBase.Destruction;
using Assets.CodeBase.GameStates;
using Assets.CodeBase.Teams;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

namespace Assets.CodeBase.Player.Respawn
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(BeginRespawnSystemGroup))]
    [UpdateBefore(typeof(RespawnVehicleCooldownSystem))]
    public partial struct RespawnVehicleStartCooldownSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (spawnedEntity, timeToRespawn, cooldown, parametersEntity)
                in SystemAPI.Query<RespawnedEntity, RefRW<TimeToRespawn>, RespawnCooldown>()
                .WithAll<RespawnedEntityIsAliveTag>()
                .WithEntityAccess()) {

                if (state.EntityManager.Exists(spawnedEntity.Value))
                    continue;

                timeToRespawn.ValueRW.Value = cooldown.Value;

                ecb.RemoveComponent<RespawnedEntityIsAliveTag>(parametersEntity);
                ecb.AddComponent<OnRespawnCooldownTag>(parametersEntity);
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(BeginRespawnSystemGroup))]
    [UpdateAfter(typeof(RespawnVehicleStartCooldownSystem))]
    [UpdateBefore(typeof(RespawnVehicleSystem))]
    public partial struct RespawnVehicleCooldownSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (timeToRespawn, entity)
                in SystemAPI.Query<RefRW<TimeToRespawn>>()
                .WithAll<OnRespawnCooldownTag>()
                .WithEntityAccess()) {

                timeToRespawn.ValueRW.Value -= SystemAPI.Time.DeltaTime;

                if (timeToRespawn.ValueRW.Value > 0)
                    continue;

                ecb.RemoveComponent<OnRespawnCooldownTag>(entity);
                ecb.AddComponent<ShouldRespawnTag>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(BeginRespawnSystemGroup))]
    [UpdateAfter(typeof(RespawnVehicleCooldownSystem))]
    public partial struct RespawnVehicleSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (respawnParameters, playerEntity)
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

                ecb.SetComponent(playerEntity, new RespawnedEntity { Value = newVehicle });
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(EndRespawnSystemGroup))]
    [UpdateBefore(typeof(RespawnedEntityCleanUpSystem))]
    public partial struct EndRespawnSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (Entity entity
                in SystemAPI.QueryBuilder()
                .WithAll<ShouldRespawnTag>()
                .Build().ToEntityArray(Allocator.Temp)) {

                ecb.RemoveComponent<ShouldRespawnTag>(entity);
                ecb.AddComponent<RespawnedEntityIsAliveTag>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(EndRespawnSystemGroup))]
    [UpdateAfter(typeof(EndRespawnSystem))]
    public partial struct RespawnedEntityCleanUpSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (spawnedEntity, parametersEntity)
                in SystemAPI.Query<RespawnedEntity>()
                .WithNone<VehicleRespawnParameters>()
                .WithEntityAccess()) {

                if (state.EntityManager.Exists(spawnedEntity.Value))
                    ecb.AddComponent<DestroyEntityTag>(spawnedEntity.Value);

                ecb.RemoveComponent<RespawnedEntity>(parametersEntity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
