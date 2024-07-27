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
                in SystemAPI.Query<WaypointSettingsReference>()
                .WithAll<InitializeMobSpawnerTag>()
                .WithEntityAccess()) {

                ref WaypointSettings waypointSettings = ref waypointReference.Blob.Value;

                UnityEngine.Debug.Log($"Team amount is {waypointSettings.TeamAmount}");

                for (int i = 0; i < 2; i++)
                    UnityEngine.Debug.Log($"Offset R {i} is {waypointSettings.RouteOffsets[i]}");

                for (int i = 0; i < 2; i++)
                    UnityEngine.Debug.Log($"Amount R {i} is {waypointSettings.RouteAmount[i]}");

                for (int i = 0; i < 4; i++)
                    UnityEngine.Debug.Log($"Offset W {i} is {waypointSettings.WaypointOffsets[i]}");

                for (int i = 0; i < 4; i++)
                    UnityEngine.Debug.Log($"Amount W {i} is {waypointSettings.WaypointAmount[i]}");

                for (int i = 0; i < 20; i++)
                    UnityEngine.Debug.Log($"Waypoint {i} is {waypointSettings.Waypoints[i]}");

                ecb.RemoveComponent<InitializeMobSpawnerTag>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
