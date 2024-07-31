using Assets.CodeBase.GameStates;
using Unity.Entities;

namespace Assets.CodeBase.Mobs.Spawn
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateAfter(typeof(MobSpawnSystem))]
    public partial struct MobRouteAdjustSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

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
