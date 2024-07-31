using Assets.CodeBase.GameStates;
using Unity.Entities;
using Unity.Transforms;

namespace Assets.CodeBase.Mobs.Spawn
{

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateAfter(typeof(SelectMobSpawnPositionSystem))]
    public partial struct MobSpawnSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (prefab, currentRoute, routeOffset, spawnPosition, waypointSettingsReference)
                in SystemAPI.Query<MobPrefab, CurrentRoute, RouteOffset, MobSpawnPosition, WaypointSettingsReference>()
                .WithAll<ShouldSpawnMobTag>()) {

                Entity mob = ecb.Instantiate(prefab.Value);

                ecb.SetComponent(mob, LocalTransform.FromPosition(spawnPosition.Value));
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
