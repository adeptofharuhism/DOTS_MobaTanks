using Assets.CodeBase.GameStates;
using Unity.Entities;

namespace Assets.CodeBase.Mobs.Spawn.Initialization
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(SpawnInitializationSystemGroup))]
    [UpdateAfter(typeof(InitialSpawnRequestProcessSystem))]
    [UpdateBefore(typeof(SpawnerInstantiationSystem))]
    public partial struct SpawnRequestProcessSystem : ISystem
    {
        private const int InitialRoute = 0;

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (waypointSettingsReference, mobSpawnerPrefab, newSpawnRequests, newSpawnerParametersBuffer)
                in SystemAPI.Query<WaypointSettingsReference, SpawnerPrefab,
                DynamicBuffer<NewSpawnRequestElement>, DynamicBuffer<NewSpawnerInstantiationParametersElement>>()) {

                ref WaypointSettings waypointSettings = ref waypointSettingsReference.Blob.Value;

                foreach (var newSpawnRequest in newSpawnRequests) {
                    ushort routeAmount = waypointSettings.RouteAmount[newSpawnRequest.Team];

                    for (int i = 0; i < routeAmount; i++)
                        newSpawnerParametersBuffer.Add(new NewSpawnerInstantiationParametersElement {
                            SpawnerPrefab = mobSpawnerPrefab.Value,
                            MobPrefab = newSpawnRequest.MobPrefab,
                            WaveCooldown = newSpawnRequest.WaveCooldown,
                            CurrentRoute = InitialRoute,
                            RouteAmount = routeAmount,
                            RouteOffset = waypointSettings.RouteOffsets[newSpawnRequest.Team],
                            Team = newSpawnRequest.Team
                        });
                }

                newSpawnRequests.Clear();
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
