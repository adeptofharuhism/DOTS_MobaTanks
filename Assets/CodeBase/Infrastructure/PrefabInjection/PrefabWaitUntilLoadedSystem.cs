using Unity.Entities;

namespace Assets.CodeBase.Infrastructure.PrefabInjection
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct PrefabWaitUntilLoadedSystem : ISystem
    {
        private const float WaitTime = 2f;

        private float _waitTimer;

        public void OnCreate(ref SystemState state) {
            _waitTimer = 0;
            
            state.RequireForUpdate<GamePrefabs>();
        }

        public void OnUpdate(ref SystemState state) {
            _waitTimer += SystemAPI.Time.DeltaTime;

            if (_waitTimer > WaitTime) {
                EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
                Entity prefabsEntity = SystemAPI.GetSingletonEntity<GamePrefabs>();

                ecb.AddComponent<TimeForPrefabsToPreparePassedTag>(prefabsEntity);

                ecb.Playback(state.EntityManager);

                state.Enabled = false;
            }
        }
    }
}
