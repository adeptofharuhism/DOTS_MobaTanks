using Assets.CodeBase.GameStates;
using Unity.Entities;

namespace Assets.CodeBase.Mobs.Spawn.Spawner
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(SpawnSystemGroup))]
    [UpdateAfter(typeof(MobRouteAdjustSystem))]
    public partial struct MobSpawnTagRemoveSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (tag, entity)
                in SystemAPI.Query<ShouldSpawnMobTag>()
                .WithEntityAccess())
                ecb.RemoveComponent<ShouldSpawnMobTag>(entity);

            ecb.Playback(state.EntityManager);
        }
    }
}
