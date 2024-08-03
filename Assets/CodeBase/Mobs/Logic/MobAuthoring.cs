using Assets.CodeBase.Combat.Teams;
using Assets.CodeBase.Mobs.Logic.Attack;
using Assets.CodeBase.Mobs.Logic.MoveToPoint;
using Assets.CodeBase.Mobs.Logic.MoveToTarget;
using Assets.CodeBase.Mobs.Logic.TargetSearch;
using Assets.CodeBase.Mobs.Spawn;
using Assets.CodeBase.Targeting;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.CodeBase.Mobs.Logic
{
    [RequireComponent(typeof(TeamAuthoring))]
    [RequireComponent(typeof(TargeterAuthoring))]
    public class MobAuthoring : MonoBehaviour
    {
        [SerializeField] private float _targetSearchInterval = 1f;
        [SerializeField] private float _requiredDistanceToWaypoint = 1f;
        [SerializeField] private float _targetChaseTime = 3f;
        [SerializeField] private float _attackCooldown = 1f;
        [SerializeField] private float _attackDistance = 5f;
        [SerializeField] private float _attackDamage = 22f;

        public float TargetSearchInterval => _targetSearchInterval;
        public float RequiredDistanceToWaypoint => _requiredDistanceToWaypoint;
        public float TargetChaseTime => _targetChaseTime;
        public float AttackCooldown => _attackCooldown;
        public float AttackDistance => _attackDistance;
        public float AttackDamage => _attackDamage;

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
                    Value = math.square(authoring.RequiredDistanceToWaypoint)
                });
                AddComponent(mob, new SquaredTargetSearchRange {
                    Value = math.square(authoring.GetComponent<TargeterAuthoring>().TargetSearchRange)
                });

                AddComponent<ChasedTarget>(mob);
                AddComponent<ChasedTargetPosition>(mob);
                AddComponent<SquaredChasedTargetDistance>(mob);
                AddComponent<ChaseTimeLeft>(mob);
                AddComponent(mob, new ChaseDuration { Value = authoring.TargetChaseTime });

                AddComponent(mob, new TargetSearchCooldown { Value = authoring._targetSearchInterval });
                AddComponent(mob, new TargetSearchCooldownTimeLeft { Value = authoring._targetSearchInterval });

                AddComponent<AttackIsOnCooldownTag>(mob);
                AddComponent(mob, new AttackDamage { Value = authoring.AttackDamage });
                AddComponent(mob, new AttackCooldown { Value = authoring.AttackCooldown });
                AddComponent(mob, new AttackCooldownTimeLeft { Value = authoring.AttackCooldown });
                AddComponent(mob, new SquaredAttackDistance {
                    Value = math.square(authoring.AttackDistance)
                });
            }
        }
    }
}
