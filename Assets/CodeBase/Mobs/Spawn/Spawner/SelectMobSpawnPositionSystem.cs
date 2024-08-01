using Assets.CodeBase.GameStates;
using Unity.Entities;

namespace Assets.CodeBase.Mobs.Spawn.Spawner
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(SpawnSystemGroup))]
    [UpdateAfter(typeof(UpdateMobSpawnCooldownSystem))]
    [UpdateBefore(typeof(MobSpawnSystem))]
    public partial struct SelectMobSpawnPositionSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

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
}
