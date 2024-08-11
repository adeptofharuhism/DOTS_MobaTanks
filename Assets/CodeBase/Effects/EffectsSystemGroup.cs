using Assets.CodeBase.Mobs;
using Unity.Entities;

namespace Assets.CodeBase.Effects
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(MobUpdateSystemGroup))]
    public partial class EffectsSystemGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(EffectsSystemGroup))]
    public partial class PostDeathEffectsSystemGroup : ComponentSystemGroup { }
}
