using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Teams
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
