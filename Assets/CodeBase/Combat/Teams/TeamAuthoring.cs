using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Combat.Teams
{
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
