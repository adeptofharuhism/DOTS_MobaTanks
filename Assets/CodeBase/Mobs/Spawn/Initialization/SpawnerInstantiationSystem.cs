using Assets.CodeBase.Combat.Teams;
using Assets.CodeBase.GameStates;
using Unity.Burst;
using Unity.Entities;

namespace Assets.CodeBase.Mobs.Spawn.Initialization
{
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
                    ecb.SetComponent(newSpawner, new MobSpawnCooldownTimeLeft { Value = 0 });

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
