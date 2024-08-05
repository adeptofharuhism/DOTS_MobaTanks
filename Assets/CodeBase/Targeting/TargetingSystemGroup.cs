using Unity.Entities;
using Unity.Physics.Systems;

namespace Assets.CodeBase.Targeting
{
    [UpdateInGroup(typeof(PhysicsSystemGroup), OrderLast = true)]
    public partial class TargetingSystemGroup : ComponentSystemGroup { }
}
