using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Combat.Teams
{
    public enum TeamType
    {
        None,
        Blue,
        Orange
    }

    public struct UnitTeam : IComponentData
    {
        [GhostField] public TeamType Value;
    }
}
