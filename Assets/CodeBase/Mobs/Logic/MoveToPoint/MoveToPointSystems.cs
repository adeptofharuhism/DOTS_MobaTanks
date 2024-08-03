using Assets.CodeBase.GameStates;
using Assets.CodeBase.Mobs.Spawn;
using Assets.CodeBase.Targeting;
using ProjectDawn.Navigation;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Assets.CodeBase.Mobs.Logic.MoveToPoint
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(MoveToPointStateSystemGroup))]
    [UpdateBefore(typeof(MoveToPointStateInitializationSystem))]
    public partial struct SetDestinationOnStateEnteranceSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            foreach (var (waypoint, agent)
                in SystemAPI.Query<CurrentWaypoint, RefRW<AgentBody>>()
                .WithAll<EnterMoveToPointState>()) {

                agent.ValueRW.SetDestination(waypoint.Value);
            }
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(MoveToPointStateSystemGroup))]
    [UpdateAfter(typeof(SetDestinationOnStateEnteranceSystem))]
    [UpdateBefore(typeof(CalculateSquaredDistanceToWaypoint))]
    public partial struct MoveToPointStateInitializationSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach(var (tag, entity)
                in SystemAPI.Query<EnterMoveToPointState>()
                .WithEntityAccess()) {

                ecb.RemoveComponent<EnterMoveToPointState>(entity);
                ecb.AddComponent<MoveToPointState>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(MoveToPointStateSystemGroup))]
    [UpdateAfter(typeof(MoveToPointStateInitializationSystem))]
    [UpdateBefore(typeof(CheckCurrentDistanceToPointSystem))]
    public partial struct CalculateSquaredDistanceToWaypoint : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            foreach (var (squaredDistance, currentWaypoint, transform)
                in SystemAPI.Query<RefRW<SquaredDistanceToWaypoint>, CurrentWaypoint, LocalToWorld>()
                .WithAll<MoveToPointState>())
                squaredDistance.ValueRW.Value = math.distancesq(currentWaypoint.Value, transform.Position);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(MoveToPointStateSystemGroup))]
    [UpdateAfter(typeof(CalculateSquaredDistanceToWaypoint))]
    [UpdateBefore(typeof(WaypointAdjustSystem))]
    public partial struct CheckCurrentDistanceToPointSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (squaredDistance, requiredSquaredDistance, entity)
                in SystemAPI.Query<SquaredDistanceToWaypoint, SquaredRequiredDistanceToWaypoint>()
                .WithAll<MoveToPointState>()
                .WithEntityAccess()) {

                if (squaredDistance.Value < requiredSquaredDistance.Value)
                    ecb.AddComponent<ShouldAdjustWaypointTag>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(MoveToPointStateSystemGroup))]
    [UpdateAfter(typeof(CheckCurrentDistanceToPointSystem))]
    [UpdateBefore(typeof(WaypointSetSystem))]
    public partial struct WaypointAdjustSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            foreach (var (currentIndex, amount)
                in SystemAPI.Query<RefRW<CurrentWaypointIndex>, WaypointAmount>()
                .WithAll<MoveToPointState, ShouldAdjustWaypointTag>()) {

                currentIndex.ValueRW.Value++;

                if (currentIndex.ValueRO.Value < amount.Value)
                    continue;

                currentIndex.ValueRW.Value = 0;
            }
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(MoveToPointStateSystemGroup))]
    [UpdateAfter(typeof(WaypointAdjustSystem))]
    [UpdateBefore(typeof(SetDestinationSystem))]
    public partial struct WaypointSetSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            foreach (var (current, currentIndex, offset, settingsReference)
                in SystemAPI.Query<RefRW<CurrentWaypoint>, CurrentWaypointIndex, WaypointOffset, WaypointSettingsReference>()
                .WithAll<MoveToPointState, ShouldAdjustWaypointTag>()) {

                ref WaypointSettings waypointSettings = ref settingsReference.Blob.Value;

                current.ValueRW.Value = waypointSettings.Waypoints[currentIndex.Value + offset.Value];
            }
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(MoveToPointStateSystemGroup))]
    [UpdateAfter(typeof(WaypointSetSystem))]
    [UpdateBefore(typeof(RemoveAdjustWaypointTagSystem))]
    public partial struct SetDestinationSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            foreach (var (waypoint, agent)
                in SystemAPI.Query<CurrentWaypoint, RefRW<AgentBody>>()
                .WithAll<MoveToPointState, ShouldAdjustWaypointTag>()) {

                agent.ValueRW.SetDestination(waypoint.Value);
            }
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(MoveToPointStateSystemGroup))]
    [UpdateAfter(typeof(SetDestinationSystem))]
    [UpdateBefore(typeof(FoundTargetEnterMoveToTargetSystem))]
    public partial struct RemoveAdjustWaypointTagSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (tag, entity)
                in SystemAPI.Query<ShouldAdjustWaypointTag>()
                .WithAll<MoveToPointState>()
                .WithEntityAccess())
                ecb.RemoveComponent<ShouldAdjustWaypointTag>(entity);

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(MoveToPointStateSystemGroup))]
    [UpdateAfter(typeof(RemoveAdjustWaypointTagSystem))]
    public partial struct FoundTargetEnterMoveToTargetSystem : ISystem {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (currentTarget, entity)
                in SystemAPI.Query<CurrentTarget>()
                .WithAll<MoveToPointState>()
                .WithEntityAccess()) {

                if (!state.EntityManager.Exists(currentTarget.Value))
                    continue;

                ecb.RemoveComponent<MoveToPointState>(entity);
                ecb.AddComponent<EnterMoveToTargetState>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
