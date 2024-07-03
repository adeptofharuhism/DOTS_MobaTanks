using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Network
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(NetworkReceiveSystemGroup))]
    public partial class NetworkProcessSystemGroup : ComponentSystemGroup
    {
    }
}
