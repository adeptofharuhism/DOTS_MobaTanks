using Assets.CodeBase.GameStates;
using Assets.CodeBase.Mobs.Logic.MoveToPoint;
using Assets.CodeBase.Teams;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Assets.CodeBase.Mobs.Spawn.Spawner
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(SpawnSystemGroup))]
    [UpdateBefore(typeof(SelectMobSpawnPositionSystem))]
    public partial struct UpdateMobSpawnCooldownSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (cooldown, cooldownTimeLeft, entity)
                in SystemAPI.Query<MobSpawnCooldown, RefRW<MobSpawnCooldownTimeLeft>>()
                .WithEntityAccess()) {

                cooldownTimeLeft.ValueRW.Value -= SystemAPI.Time.DeltaTime;

                if (cooldownTimeLeft.ValueRO.Value > 0)
                    continue;

                cooldownTimeLeft.ValueRW.Value = cooldown.Value;

                ecb.AddComponent<ShouldSpawnMobTag>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(SpawnSystemGroup))]
    [UpdateAfter(typeof(UpdateMobSpawnCooldownSystem))]
    [UpdateBefore(typeof(MobSpawnSystem))]
    public partial struct SelectMobSpawnPositionSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (currentRoute, routeOffset, mobSpawnPosition, waypointSettingsReference)
                in SystemAPI.Query<CurrentRoute, RouteOffset, RefRW<MobSpawnPosition>, WaypointSettingsReference>()
                .WithAll<ShouldSpawnMobTag>()) {

                ref WaypointSettings waypointSettings = ref waypointSettingsReference.Blob.Value;

                mobSpawnPosition.ValueRW.Value =
                    waypointSettings.Waypoints[waypointSettings.WaypointOffsets[currentRoute.Value + routeOffset.Value]];
            }
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(SpawnSystemGroup))]
    [UpdateAfter(typeof(SelectMobSpawnPositionSystem))]
    [UpdateBefore(typeof(MobRouteAdjustSystem))]
    public partial struct MobSpawnSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (prefab, currentRoute, routeOffset, spawnPosition, waypointSettingsReference, team)
                in SystemAPI.Query<MobPrefab, CurrentRoute, RouteOffset, MobSpawnPosition, WaypointSettingsReference, UnitTeam>()
                .WithAll<ShouldSpawnMobTag>()) {

                ref WaypointSettings waypointSettings = ref waypointSettingsReference.Blob.Value;

                Entity mob = ecb.Instantiate(prefab.Value);

                ecb.SetComponent(mob, LocalTransform.FromPosition(spawnPosition.Value));

                ecb.SetComponent(mob, new WaypointSettingsReference { Blob = waypointSettingsReference.Blob });

                int routeIndex = currentRoute.Value + routeOffset.Value;
                ecb.SetComponent(mob, new CurrentWaypointIndex { Value = 0 });
                ecb.SetComponent(mob, new WaypointAmount { Value = waypointSettings.WaypointAmount[routeIndex] });
                ecb.SetComponent(mob, new WaypointOffset { Value = waypointSettings.WaypointOffsets[routeIndex] });
                ecb.SetComponent(mob, new CurrentWaypoint {
                    Value = waypointSettings.Waypoints[waypointSettings.WaypointOffsets[routeIndex]]
                });

                ecb.SetComponent(mob, new UnitTeam { Value = team.Value });
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(SpawnSystemGroup))]
    [UpdateAfter(typeof(MobSpawnSystem))]
    [UpdateBefore(typeof(MobSpawnTagRemoveSystem))]
    public partial struct MobRouteAdjustSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (currentRoute, routeAmount)
                in SystemAPI.Query<RefRW<CurrentRoute>, RouteAmount>()
                .WithAll<ShouldSpawnMobTag>()) {

                currentRoute.ValueRW.Value++;

                if (currentRoute.ValueRO.Value < routeAmount.Value)
                    continue;

                currentRoute.ValueRW.Value = 0;
            }
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(SpawnSystemGroup))]
    [UpdateAfter(typeof(MobRouteAdjustSystem))]
    public partial struct MobSpawnTagRemoveSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (tag, entity)
                in SystemAPI.Query<ShouldSpawnMobTag>()
                .WithEntityAccess())
                ecb.RemoveComponent<ShouldSpawnMobTag>(entity);

            ecb.Playback(state.EntityManager);
        }
    }
}
