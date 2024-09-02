using Assets.CodeBase.Teams;
using Unity.Entities;

namespace Assets.CodeBase.GameStates.InGame
{
    public struct WinnerTeam : IComponentData
    {
        public TeamType Value;
    }
}
