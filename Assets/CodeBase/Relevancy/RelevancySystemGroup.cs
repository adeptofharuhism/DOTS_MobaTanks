using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Relevancy
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(GhostSimulationSystemGroup))]
    public partial class RelevancySystemGroup : ComponentSystemGroup { }
}
