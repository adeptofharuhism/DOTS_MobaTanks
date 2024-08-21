using Assets.CodeBase.Effects;
using Unity.Entities;

namespace Assets.CodeBase.Finances
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(EffectsSystemGroup))]
    public partial class FinancesSystemGroup : ComponentSystemGroup { }
}
