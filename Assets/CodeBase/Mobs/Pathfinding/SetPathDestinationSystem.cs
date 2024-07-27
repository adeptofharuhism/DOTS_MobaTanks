using ProjectDawn.Navigation;
using Unity.Entities;

namespace Assets.CodeBase.Mobs.Pathfinding
{
    public partial struct SetPathDestinationSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            foreach (var (destination, agent)
                in SystemAPI.Query<PathDestination, RefRW<AgentBody>>()) {

                agent.ValueRW.SetDestination(destination.Value);
            }
        }
    }
}
