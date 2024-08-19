using Assets.CodeBase.GameStates;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Assets.CodeBase.Effects.PostDeath
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(PostDeathEffectsSystemGroup))]
    [UpdateBefore(typeof(UpdatePositionForPostDeathEffectSystem))]
    public partial struct InitializePostDeathEffectSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach(var (initializationData, entity)
                in SystemAPI.Query<PostDeathEffectInitializationData>()
                .WithEntityAccess()) {

                ecb.AddComponent<PostDeathEffectPosition>(entity);
                ecb.AddComponent<PostDeathEffectRotation>(entity);
                ecb.AddComponent(entity, new PostDeathEffectPrefab { Value = initializationData.EffectPfefab });

                ecb.RemoveComponent<PostDeathEffectInitializationData>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(PostDeathEffectsSystemGroup))]
    [UpdateAfter(typeof(InitializePostDeathEffectSystem))]
    [UpdateBefore(typeof(UpdateRotationForPostDeathEffectSystem))]
    public partial struct UpdatePositionForPostDeathEffectSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (transform, effectPosition)
                in SystemAPI.Query<LocalTransform, RefRW<PostDeathEffectPosition>>())

                effectPosition.ValueRW.Value = transform.Position;
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(PostDeathEffectsSystemGroup))]
    [UpdateAfter(typeof(UpdatePositionForPostDeathEffectSystem))]
    [UpdateBefore(typeof(SpawnPostDeathEffectSystem))]
    public partial struct UpdateRotationForPostDeathEffectSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (transform, effectRotation)
                in SystemAPI.Query<LocalTransform, RefRW<PostDeathEffectRotation>>())

                effectRotation.ValueRW.Value = transform.Rotation;
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(PostDeathEffectsSystemGroup))]
    [UpdateAfter(typeof(UpdateRotationForPostDeathEffectSystem))]
    [UpdateBefore(typeof(RemovePostDeathComponentsSystem))]
    public partial struct SpawnPostDeathEffectSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new(Allocator.Temp);

            foreach (var (effectPrefab, position, rotation)
                in SystemAPI.Query<PostDeathEffectPrefab, PostDeathEffectPosition, PostDeathEffectRotation>()
                .WithNone<LocalTransform>()) {

                Entity effect = ecb.Instantiate(effectPrefab.Value);

                ecb.SetComponent(effect, LocalTransform.FromPositionRotation(position.Value, rotation.Value));
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(PostDeathEffectsSystemGroup))]
    [UpdateAfter(typeof(SpawnPostDeathEffectSystem))]
    public partial struct RemovePostDeathComponentsSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new(Allocator.Temp);

            foreach (Entity entity
                in SystemAPI.QueryBuilder()
                .WithAll<PostDeathEffectPrefab, PostDeathEffectPosition, PostDeathEffectRotation>()
                .WithNone<LocalTransform>().Build().ToEntityArray(Allocator.Temp)) {

                ecb.RemoveComponent<PostDeathEffectPrefab>(entity);
                ecb.RemoveComponent<PostDeathEffectPosition>(entity);
                ecb.RemoveComponent<PostDeathEffectRotation>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
