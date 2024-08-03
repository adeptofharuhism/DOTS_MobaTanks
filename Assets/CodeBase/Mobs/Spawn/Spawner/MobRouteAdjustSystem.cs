using Assets.CodeBase.GameStates;
using Unity.Burst;
using Unity.Entities;

namespace Assets.CodeBase.Mobs.Spawn.Spawner
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(SpawnSystemGroup))]
    [UpdateAfter(typeof(MobSpawnSystem))]
    [UpdateBefore(typeof(MobSpawnTagRemoveSystem))]
    public partial struct MobRouteAdjustSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (currentRoute, routeAmount)
                in SystemAPI.Query<RefRW<CurrentRoute>, RouteAmount>()
                .WithAll<ShouldSpawnMobTag>()) {

                currentRoute.ValueRW.Value++;

                if (currentRoute.ValueRO.Value < routeAmount.Value)
                    continue;

                currentRoute.ValueRW.Value = 0;
            }
        }
    }
}
