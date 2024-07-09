using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.GameStates
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(NetworkReceiveSystemGroup))]
    public partial class GameStateSystemGroup : ComponentSystemGroup
    {
    }

    [UpdateInGroup(typeof(GameStateSystemGroup))]
    [UpdateBefore(typeof(ReportInGameStateSystemGroup))]
    public partial class PrepareForGameStateSystemGroup : ComponentSystemGroup
    {
    }

    [UpdateInGroup(typeof(GameStateSystemGroup))]
    [UpdateAfter(typeof(PrepareForGameStateSystemGroup))]
    [UpdateBefore(typeof(InGameStateSystemGroup))]
    public partial class ReportInGameStateSystemGroup : ComponentSystemGroup
    {
    }

    [UpdateInGroup(typeof(GameStateSystemGroup))]
    [UpdateAfter(typeof(ReportInGameStateSystemGroup))]
    [UpdateBefore(typeof(ReportEndGameStateSystemGroup))]
    public partial class InGameStateSystemGroup : ComponentSystemGroup
    {
    }

    [UpdateInGroup(typeof(GameStateSystemGroup))]
    [UpdateAfter(typeof(InGameStateSystemGroup))]
    [UpdateBefore(typeof(EndGameStateSystemGroup))]
    public partial class ReportEndGameStateSystemGroup : ComponentSystemGroup
    {
    }

    [UpdateInGroup(typeof(GameStateSystemGroup))]
    [UpdateAfter(typeof(ReportEndGameStateSystemGroup))]
    public partial class EndGameStateSystemGroup : ComponentSystemGroup
    {
    }
}
