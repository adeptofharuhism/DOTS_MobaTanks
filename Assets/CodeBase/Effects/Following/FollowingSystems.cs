using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Assets.CodeBase.Effects.Following
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(EffectsSystemGroup))]
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
