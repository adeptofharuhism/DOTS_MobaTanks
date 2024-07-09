using Assets.CodeBase.Combat.Teams;
using Unity.Entities;

namespace Assets.CodeBase.GameStates.InGame
{
    public struct WinnerTeam : IComponentData
    {
        public TeamType Value;
    }
}
