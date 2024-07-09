using Assets.CodeBase.Combat.Teams;
using Unity.NetCode;

namespace Assets.CodeBase.GameStates.ReportEndGame
{
    public struct GoToEndGameStateRpc : IRpcCommand
    {
        public TeamType Winner;
    }
}
