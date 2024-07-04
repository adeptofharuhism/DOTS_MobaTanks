using Assets.CodeBase.Combat.Teams;
using Assets.CodeBase.Network.GameStart;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace Assets.CodeBase.Targeting
{
    [UpdateInGroup(typeof(PhysicsSystemGroup), OrderLast = true)]
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
            state.RequireForUpdate<InGame>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            CollisionWorld collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

            foreach (var (range, targeterTransform, currentTarget, team)
                in SystemAPI.Query<TargeterRange, RefRO<LocalToWorld>, RefRW<CurrentTarget>, UnitTeam>()
                .WithAll<Targeter>()) {

                NativeList<DistanceHit> distanceHits = new NativeList<DistanceHit>(Allocator.Temp);

                collisionWorld.OverlapSphere(
                    targeterTransform.ValueRO.Position,
                    range.Value,
                    ref distanceHits,
                    _collisionFilter,
                    QueryInteraction.IgnoreTriggers);

                RemoveAlliedUnits(ref state, ref distanceHits, team);

                Entity targetEntity = SelectRandomTarget(ref distanceHits);
                currentTarget.ValueRW.Value = SelectTargetingPoint(ref state, targetEntity);
            }
        }

        [BurstCompile]
        private void RemoveAlliedUnits(ref SystemState state, ref NativeList<DistanceHit> distanceHits, UnitTeam team) {
            for (int i = 0; i < distanceHits.Length;) {
                UnitTeam targetTeam = SystemAPI.GetComponent<UnitTeam>(distanceHits[i].Entity);
                if (team.Value == targetTeam.Value)
                    distanceHits.RemoveAt(i);
                else
                    i++;
            }
        }

        [BurstCompile]
        private Entity SelectRandomTarget(ref NativeList<DistanceHit> distanceHits) {
            if (distanceHits.Length == 0)
                return Entity.Null;

            int targetIndex = _random.NextInt(distanceHits.Length - 1);
            return distanceHits[targetIndex].Entity;
        }

        [BurstCompile]
        private Entity SelectTargetingPoint(ref SystemState state, Entity entity) {
            if (SystemAPI.HasComponent<TargetPosition>(entity))
                return SystemAPI.GetComponent<TargetPosition>(entity).Value;

            return entity;
        }
    }
}
