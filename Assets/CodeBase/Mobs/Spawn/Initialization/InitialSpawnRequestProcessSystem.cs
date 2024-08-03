using Assets.CodeBase.GameStates;
using Unity.Burst;
using Unity.Entities;

namespace Assets.CodeBase.Mobs.Spawn.Initialization
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(SpawnInitializationSystemGroup))]
    [UpdateBefore(typeof(SpawnRequestProcessSystem))]
    public partial struct InitialSpawnRequestProcessSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
            state.RequireForUpdate<InitializeSpawnRequestProcessTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (waypointSettingsReference, mobSpawnerPrefab, newSpawnRequests, newSpawnerParametersBuffer, entity)
                in SystemAPI.Query<WaypointSettingsReference, SpawnerPrefab,
                DynamicBuffer<NewSpawnRequestElement>, DynamicBuffer<NewSpawnerInstantiationParametersElement>>()
                .WithAll<InitializeSpawnRequestProcessTag>()
                .WithEntityAccess()) {

                ref WaypointSettings waypointSettings = ref waypointSettingsReference.Blob.Value;

                foreach (var newSpawnRequest in newSpawnRequests) {
                    ushort routeAmount = waypointSettings.RouteAmount[newSpawnRequest.Team];

                    for (int i = 0; i < routeAmount; i++)
                        newSpawnerParametersBuffer.Add(new NewSpawnerInstantiationParametersElement {
                            SpawnerPrefab = mobSpawnerPrefab.Value,
                            MobPrefab = newSpawnRequest.MobPrefab,
                            WaveCooldown = newSpawnRequest.WaveCooldown,
                            CurrentRoute = (ushort)i,
                            RouteAmount = routeAmount,
                            RouteOffset = waypointSettings.RouteOffsets[newSpawnRequest.Team],
                            Team = newSpawnRequest.Team
                        });
                }

                newSpawnRequests.Clear();

                ecb.RemoveComponent<InitializeSpawnRequestProcessTag>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
