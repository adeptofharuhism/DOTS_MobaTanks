using Assets.CodeBase.GameStates;
using Assets.CodeBase.GameStates.InGame;
using Assets.CodeBase.Infrastructure.PrefabInjection;
using Assets.CodeBase.Teams;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Assets.CodeBase.Structures.Bases
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(BaseStructureSystemGroup))]
    [UpdateBefore(typeof(BaseCleanUpSystem))]
    public partial struct BaseSpawnSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
            state.RequireForUpdate<GamePrefabs>();
            state.RequireForUpdate<BaseSpawnPositions>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            Entity basePrefab = SystemAPI.GetSingleton<GamePrefabs>().Base;

            foreach (var (spawnPositions, entity)
                in SystemAPI.Query<BaseSpawnPositions>()
                .WithEntityAccess()) {

                SpawnBase(ref state, ref ecb, basePrefab, spawnPositions.BlueBase, TeamType.Blue);
                SpawnBase(ref state, ref ecb, basePrefab, spawnPositions.OrangeBase, TeamType.Orange);

                ecb.DestroyEntity(entity);
            }

            ecb.Playback(state.EntityManager);
        }

        private void SpawnBase(ref SystemState state, ref EntityCommandBuffer ecb, Entity basePrefab, float3 spawnPosition, TeamType team) {
            Entity baseEntity = ecb.Instantiate(basePrefab);

            ecb.SetComponent(baseEntity, LocalTransform.FromPosition(spawnPosition));

            ecb.SetComponent(baseEntity, new UnitTeam { Value = team });

            ecb.AddComponent<BaseTag>(baseEntity);
            ecb.AddComponent(baseEntity, new BaseTeamCleanUp { Team = team });
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(BaseStructureSystemGroup))]
    [UpdateAfter(typeof(BaseSpawnSystem))]
    public partial struct BaseCleanUpSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (loserTeam, entity)
                in SystemAPI.Query<BaseTeamCleanUp>()
                .WithNone<BaseTag>()
                .WithEntityAccess()) {

                Entity winningTeamEntity = ecb.CreateEntity();
                ecb.AddComponent(winningTeamEntity, new WinnerTeam {
                    Value = loserTeam.Team == TeamType.Blue ? TeamType.Orange : TeamType.Blue
                });

                ecb.RemoveComponent<BaseTeamCleanUp>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
