using Unity.Collections;
using Unity.Entities;
using Unity.Scenes;

namespace Assets.CodeBase.Infrastructure.PrefabInjection
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct SubSceneLoadWaitSystem : ISystem
    {
        private EntityQuery _pendingSubScenesQuery;

        public void OnCreate(ref SystemState state) {
            _pendingSubScenesQuery = SystemAPI.QueryBuilder().WithAll<RequestSceneLoaded, SceneReference>().Build();

            state.RequireForUpdate<GamePrefabs>();
        }

        public void OnUpdate(ref SystemState state) {
            NativeArray<Entity> pendingSubScenes = _pendingSubScenesQuery.ToEntityArray(Allocator.Temp);

            foreach (Entity subScene in pendingSubScenes)
                if (!SceneSystem.IsSceneLoaded(state.WorldUnmanaged, subScene))
                    return;

            UnityEngine.Debug.Log("All sub scenes loaded");

            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
            Entity prefabsEntity = SystemAPI.GetSingletonEntity<GamePrefabs>();

            ecb.AddComponent<SubScenesLoadPassedTag>(prefabsEntity);
            ecb.AddComponent<ReadyForUIDeployTag>(prefabsEntity);

            ecb.Playback(state.EntityManager);

            state.Enabled = false;
        }
    }
}
