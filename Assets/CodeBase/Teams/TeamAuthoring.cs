using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Assets.CodeBase.Teams
{
    public enum TeamType
    {
        None = 0,
        Blue = 1,
        Orange = 2
    }

    public struct UnitTeam : IComponentData
    {
        [GhostField] public TeamType Value;
    }

    public class TeamAuthoring : MonoBehaviour
    {
        [SerializeField] private TeamType _teamType;

        public TeamType TeamType => _teamType;

        public class TeamBaker : Baker<TeamAuthoring>
        {
            public override void Bake(TeamAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new UnitTeam { Value = authoring.TeamType });
            }
        }
    }
}