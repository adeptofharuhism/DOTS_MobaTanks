using Unity.Entities;
using Unity.Transforms;

namespace Assets.CodeBase.Combat.Health
{
    [UpdateInGroup(typeof(TransformSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial class HealthBarClientSystemGroup : ComponentSystemGroup
    {
    }
}
