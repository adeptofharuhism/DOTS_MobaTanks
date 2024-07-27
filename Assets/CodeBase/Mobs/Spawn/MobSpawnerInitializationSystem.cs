using Unity.Entities;

namespace Assets.CodeBase.Mobs.Spawn
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct MobSpawnerInitializationSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InitializeMobSpawnerTag>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (waypointReference, entity)
                in SystemAPI.Query<WaypointSetReference>()
                .WithAll<InitializeMobSpawnerTag>()
                .WithEntityAccess()) {

                ref WaypointSet waypoints = ref waypointReference.Blob.Value;

                for (int i = 0; i < waypoints.Waypoints.Length; i++) {
                    UnityEngine.Debug.Log(waypoints.Waypoints[i]);
                }

                ecb.RemoveComponent<InitializeMobSpawnerTag>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
