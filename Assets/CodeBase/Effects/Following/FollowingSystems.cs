using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Assets.CodeBase.Effects.Following
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(EffectsSystemGroup))]
    public partial struct LinkToTargetSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<FollowTarget>();
            state.RequireForUpdate<LinkToTarget>();
        }

        public void OnUpdate(ref SystemState state) {
            foreach (var (followTarget, entity)
                in SystemAPI.Query<FollowTarget>()
                    .WithAll<LinkToTarget>()
                    .WithEntityAccess()) {

                bool hasBuffer = SystemAPI.HasBuffer<LinkedEntityGroup>(followTarget.Value);

                LinkedEntityGroup link = new() { Value = entity };

                if (hasBuffer)
                    SystemAPI.GetBuffer<LinkedEntityGroup>(followTarget.Value).Add(link);
                else {
                    DynamicBuffer<LinkedEntityGroup> newBuffer =
                        state.EntityManager.AddBuffer<LinkedEntityGroup>(followTarget.Value);

                    newBuffer.Add(link);
                }
            }

            EntityQuery cleanupQuery = SystemAPI.QueryBuilder().WithAll<LinkToTarget>().Build();
            state.EntityManager.RemoveComponent<LinkToTarget>(cleanupQuery);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(EffectsSystemGroup))]
    [UpdateAfter(typeof(LinkToTargetSystem))]
    public partial struct FollowTargetSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<FollowTarget>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            ComponentLookup<LocalTransform> transforms = SystemAPI.GetComponentLookup<LocalTransform>(true);

            foreach (var (target, transform)
                in SystemAPI.Query<FollowTarget, RefRW<LocalTransform>>())
                transform.ValueRW.Position = transforms[target.Value].Position;

            foreach (var (offset, transform)
                in SystemAPI.Query<FollowOffset, RefRW<LocalTransform>>())
                transform.ValueRW.Position = transform.ValueRO.Position + offset.Value;
        }
    }
}