using Assets.CodeBase.Combat.Teams;
using Assets.CodeBase.GameStates;
using Assets.CodeBase.Mobs.Logic.MoveToPoint;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Assets.CodeBase.Mobs.Spawn.Spawner
{

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
}
