using Unity.Entities;
using Unity.Physics.Systems;

namespace Assets.CodeBase
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(PhysicsSimulationGroup))]
    public partial class WheelInputsReadSystemGroup : ComponentSystemGroup { }



    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(WheelInputsReadSystemGroup))]
    public partial class WheelPhysicsSystemGroup : ComponentSystemGroup { }



    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(WheelPhysicsSystemGroup))]
    public partial class ProjectileSystemGroup : ComponentSystemGroup { }



    [UpdateInGroup(typeof(PhysicsSystemGroup), OrderLast = true)]
    public partial class TargetingSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(TargetingSystemGroup))]
    public partial class TargetingActivationSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(TargetingSystemGroup))]
    [UpdateAfter(typeof(TargetingActivationSystemGroup))]
    public partial class TargetSearchSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(TargetingSystemGroup))]
    [UpdateAfter(typeof(TargetSearchSystemGroup))]
    public partial class TargetingDeactivationSystemGroup : ComponentSystemGroup { }
}
