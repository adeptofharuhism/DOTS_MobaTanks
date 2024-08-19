using Assets.CodeBase.Combat.Teams;
using Assets.CodeBase.GameStates;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Assets.CodeBase.Targeting
{
    [UpdateInGroup(typeof(TargetingSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct TargetSearchSystem : ISystem
    {
        private const int RandomSeed = 42;

        private CollisionFilter _collisionFilter;
        private Random _random;

        public void OnCreate(ref SystemState state) {
            _collisionFilter = new CollisionFilter {
                BelongsTo = 1 << 5,
                CollidesWith = 1 << 4
            };

            _random = Random.CreateFromIndex(RandomSeed);

            state.RequireForUpdate<PhysicsWorldSingleton>();
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            CollisionWorld collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

            foreach (var (range, targeterTransform, currentTarget, team, entity)
                in SystemAPI.Query<TargeterRange, RefRO<LocalToWorld>, RefRW<CurrentTarget>, UnitTeam>()
                .WithAll<Targeter>()
                .WithEntityAccess()) {

                NativeList<DistanceHit> distanceHits = new NativeList<DistanceHit>(Allocator.Temp);

                collisionWorld.OverlapSphere(
                    targeterTransform.ValueRO.Position,
                    range.Value,
                    ref distanceHits,
                    _collisionFilter,
                    QueryInteraction.IgnoreTriggers);

                RemoveAlliedUnits(ref state, ref distanceHits, team);

                currentTarget.ValueRW.Value = SelectRandomTarget(ref distanceHits);
            }
        }

        private void RemoveAlliedUnits(ref SystemState state, ref NativeList<DistanceHit> distanceHits, UnitTeam team) {
            for (int i = 0; i < distanceHits.Length;) {
                UnitTeam targetTeam = SystemAPI.GetComponent<UnitTeam>(distanceHits[i].Entity);
                if (team.Value == targetTeam.Value)
                    distanceHits.RemoveAt(i);
                else
                    i++;
            }
        }

        private Entity SelectRandomTarget(ref NativeList<DistanceHit> distanceHits) {
            if (distanceHits.Length == 0)
                return Entity.Null;

            int targetIndex = _random.NextInt(distanceHits.Length - 1);
            return distanceHits[targetIndex].Entity;
        }
    }

    [UpdateInGroup(typeof(TargetingSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateAfter(typeof(TargetSearchSystem))]
    public partial struct TargetingPointSelectionSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<PhysicsWorldSingleton>();
            state.RequireForUpdate<InGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            foreach (RefRW<CurrentTarget> target
                in SystemAPI.Query<RefRW<CurrentTarget>>()
                .WithAll<TargetOnCertainPointTag>()) {

                if (!SystemAPI.GetComponentLookup<TargetPoint>(true)
                    .TryGetComponent(target.ValueRO.Value, out TargetPoint targetPoint))
                    continue;

                target.ValueRW.Value = targetPoint.Value;
            }
        }
    }
}
