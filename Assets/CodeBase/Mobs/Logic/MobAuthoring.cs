using Assets.CodeBase.Animation;
using Assets.CodeBase.Combat.Teams;
using Assets.CodeBase.Mobs.Logic.Animation;
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
        [Header("Waypoint follow")]
        [SerializeField] private float _requiredDistanceToWaypoint = 1f;
        [Header("Chasing")]
        [SerializeField] private float _targetSearchInterval = 1f;
        [SerializeField] private float _targetChaseTime = 3f;
        [Header("Attack")]
        [SerializeField] private float _attackCooldown = 1f;
        [SerializeField] private float _attackDistance = 5f;
        [SerializeField] private float _attackDamage = 22f;
        [Header("Animation")]
        [SerializeField] private bool _enableAnimations = false;
        [SerializeField] private MobAnimationSet _mobAnimationSet;

        public float RequiredDistanceToWaypoint => _requiredDistanceToWaypoint;

        public float TargetSearchInterval => _targetSearchInterval;
        public float TargetChaseTime => _targetChaseTime;

        public float AttackCooldown => _attackCooldown;
        public float AttackDistance => _attackDistance;
        public float AttackDamage => _attackDamage;

        public bool AnimationsEnabled => _enableAnimations;
        public MobAnimationSet MobAnimationSet => _mobAnimationSet;

        public class MobBaker : Baker<MobAuthoring>
        {
            public override void Bake(MobAuthoring authoring) {
                Entity mob = GetEntity(TransformUsageFlags.Dynamic);

                SetupStateTags(mob);
                SetupMoveToPointTags(mob);
                SetupMoveToTargetTags(mob);
                SetupAttackTags(mob);

                if (authoring.AnimationsEnabled)
                    SetupAnimations(mob, authoring.MobAnimationSet);

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

                AddComponent<MobReadyToSearchTargetTag>(mob);
                SetComponentEnabled<MobReadyToSearchTargetTag>(mob, false);
                AddComponent(mob, new TargetSearchCooldown { Value = authoring._targetSearchInterval });
                AddComponent(mob, new TargetSearchCooldownTimeLeft { Value = authoring._targetSearchInterval });

                AddComponent(mob, new AttackDamage { Value = authoring.AttackDamage });
                AddComponent(mob, new AttackCooldown { Value = authoring.AttackCooldown });
                AddComponent(mob, new AttackCooldownTimeLeft { Value = authoring.AttackCooldown });
                AddComponent(mob, new SquaredAttackDistance {
                    Value = math.square(authoring.AttackDistance)
                });
            }

            private void SetupStateTags(Entity entity) {
                AddComponent<EnterMoveToPointState>(entity);
                AddComponent<MoveToPointState>(entity);
                AddComponent<EnterMoveToTargetState>(entity);
                AddComponent<MoveToTargetState>(entity);
                AddComponent<EnterAttackState>(entity);
                AddComponent<AttackState>(entity);

                SetComponentEnabled<EnterMoveToPointState>(entity, true);
                SetComponentEnabled<MoveToPointState>(entity, false);
                SetComponentEnabled<EnterMoveToTargetState>(entity, false);
                SetComponentEnabled<MoveToTargetState>(entity, false);
                SetComponentEnabled<EnterAttackState>(entity, false);
                SetComponentEnabled<AttackState>(entity, false);
            }

            private void SetupMoveToPointTags(Entity entity) {
                AddComponent<ShouldAdjustWaypointTag>(entity);

                SetComponentEnabled<ShouldAdjustWaypointTag>(entity, false);
            }

            private void SetupMoveToTargetTags(Entity entity) {
                AddComponent<SearchForNewTargetTag>(entity);
                AddComponent<HasTargetInRangeTag>(entity);

                SetComponentEnabled<SearchForNewTargetTag>(entity, false);
                SetComponentEnabled<HasTargetInRangeTag>(entity, false);
            }

            private void SetupAttackTags(Entity entity) {
                AddComponent<AttackIsOnCooldownTag>(entity);
                AddComponent<AttackIsReadyTag>(entity);

                SetComponentEnabled<AttackIsOnCooldownTag>(entity, true);
                SetComponentEnabled<AttackIsReadyTag>(entity, false);
            }

            private void SetupAnimations(Entity mob, MobAnimationSet animationSet) {
                bool isMoving = true;
                bool isAttacking = false;

                AddComponent(mob, new IsMoving { Value = isMoving });
                AddComponent(mob, new IsAttacking { Value = isAttacking });

                AddComponent(mob, new PreviousIsMoving { Value = isMoving });
                AddComponent(mob, new PreviousIsAttacking { Value = isAttacking });

                AddComponent(mob, new IdleClipIndex { Value = animationSet.Idle });
                AddComponent(mob, new RunClipIndex { Value = animationSet.Run });
                AddComponent(mob, new AttackClipIndex { Value = animationSet.Attack });
            }
        }
    }
}
