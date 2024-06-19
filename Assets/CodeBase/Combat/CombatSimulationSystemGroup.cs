using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Combat
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    [UpdateAfter(typeof(PredictedFixedStepSimulationSystemGroup))]
    public partial class CombatSimulationSystemGroup : ComponentSystemGroup
    {
    }
}
