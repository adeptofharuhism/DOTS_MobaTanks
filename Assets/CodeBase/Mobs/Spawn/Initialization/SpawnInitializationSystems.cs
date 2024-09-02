using Assets.CodeBase.GameStates;
using Assets.CodeBase.Teams;
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

        [BurstCompile]
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

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(SpawnInitializationSystemGroup))]
    [UpdateAfter(typeof(SpawnRequestProcessSystem))]
    public partial struct SpawnerInstantiationSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (newSpawnerParametersBuffer, waypointSettingsReference)
                in SystemAPI.Query<DynamicBuffer<NewSpawnerInstantiationParametersElement>, WaypointSettingsReference>()) {

                foreach (NewSpawnerInstantiationParametersElement newSpawnerParameters in newSpawnerParametersBuffer) {
                    Entity newSpawner = ecb.Instantiate(newSpawnerParameters.SpawnerPrefab);

                    ecb.SetComponent(newSpawner, new MobPrefab { Value = newSpawnerParameters.MobPrefab });

                    ecb.SetComponent(newSpawner, new MobSpawnCooldown { Value = newSpawnerParameters.WaveCooldown });
                    ecb.SetComponent(newSpawner, new MobSpawnCooldownTimeLeft { Value = newSpawnerParameters.WaveCooldown });

                    ecb.SetComponent(newSpawner, new CurrentRoute { Value = newSpawnerParameters.CurrentRoute });
                    ecb.SetComponent(newSpawner, new RouteAmount { Value = newSpawnerParameters.RouteAmount });
                    ecb.SetComponent(newSpawner, new RouteOffset { Value = newSpawnerParameters.RouteOffset });

                    ecb.SetComponent(newSpawner, new UnitTeam { Value = GetTeamType(newSpawnerParameters.Team) });
                    ecb.SetComponent(newSpawner, new WaypointSettingsReference { Blob = waypointSettingsReference.Blob });
                }

                newSpawnerParametersBuffer.Clear();
            }

            ecb.Playback(state.EntityManager);
        }

        private TeamType GetTeamType(ushort team) =>
            team == 0 ? TeamType.Blue : TeamType.Orange;
    }
}
