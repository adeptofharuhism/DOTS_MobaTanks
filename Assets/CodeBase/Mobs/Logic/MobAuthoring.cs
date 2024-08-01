using Assets.CodeBase.Combat.Teams;
using Assets.CodeBase.Mobs.Logic.MoveToPoint;
using Assets.CodeBase.Mobs.Logic.MoveToTarget;
using Assets.CodeBase.Mobs.Spawn;
using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Mobs.Logic
{
    [RequireComponent(typeof(TeamAuthoring))]
    public class MobAuthoring : MonoBehaviour
    {
        [SerializeField] private float _requiredDistanceToWaypoint = 1f;
        [SerializeField] private float _targetChaseDistance = 60f;

        public float RequiredDistanceToWaypoint => _requiredDistanceToWaypoint;
        public float TargetChaseDistance => _targetChaseDistance;

        public class MobBaker : Baker<MobAuthoring>
        {
            public override void Bake(MobAuthoring authoring) {
                Entity mob = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<EnterMoveToPointState>(mob);

                AddComponent<WaypointSettingsReference>(mob);

                AddComponent<WaypointOffset>(mob);
                AddComponent<WaypointAmount>(mob);
                AddComponent<CurrentWaypointIndex>(mob);
                AddComponent<CurrentWaypoint>(mob);

                AddComponent<SquaredDistanceToWaypoint>(mob);
                AddComponent(mob, new SquaredRequiredDistanceToWaypoint {
                    Value = authoring.RequiredDistanceToWaypoint * authoring.RequiredDistanceToWaypoint
                });

                AddComponent<ChasedTarget>(mob);
                AddComponent<ChasedTargetPosition>(mob);
                AddComponent(mob, new SquaredChasedTargetDistance {
                    Value = authoring.TargetChaseDistance * authoring.TargetChaseDistance
                });
            }
        }
    }
}
