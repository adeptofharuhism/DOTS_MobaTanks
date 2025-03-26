using Assets.CodeBase.Effects.Coloring;
using Assets.CodeBase.Effects.Following;
using Assets.CodeBase.Infrastructure.PrefabInjection;
using Assets.CodeBase.Teams;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace Assets.CodeBase.Combat.Health.UI
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(HealthBarClientSystemGroup))]
    public partial struct HealthInitializationSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<GamePrefabs>();
            state.RequireForUpdate<HealthInitializationTag>();
        }

        public void OnUpdate(ref SystemState state) {
            Entity healthBar = SystemAPI.GetSingleton<GamePrefabs>().HealthBar;

            foreach (var (healthBarOffset, team, referenceToFillArea, entity)
                in SystemAPI.Query<HealthBarOffset, UnitTeam, RefRW<ReferenceToHealthBarFillArea>>()
                    .WithAll<HealthInitializationTag>()
                    .WithEntityAccess()) {

                Entity newHealthBar = state.EntityManager.Instantiate(healthBar);

                state.EntityManager.SetComponentData(newHealthBar, new FollowTarget() { Value = entity });
                state.EntityManager.SetComponentData(newHealthBar, new FollowOffset() { Value = healthBarOffset.Value });
                state.EntityManager.SetComponentData(newHealthBar, new UnitTeam() { Value = team.Value });

                referenceToFillArea.ValueRW.Value = SystemAPI.GetComponent<HealthBarParts>(newHealthBar).Fill;
            }

            EntityQuery healths = SystemAPI.QueryBuilder().WithAll<HealthInitializationTag>().Build();
            state.EntityManager.RemoveComponent<HealthInitializationTag>(healths);
            state.EntityManager.RemoveComponent<HealthBarOffset>(healths);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(HealthBarClientSystemGroup))]
    [UpdateAfter(typeof(HealthInitializationSystem))]
    public partial struct HealthBarInitializationSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<HealthBarInitializeTag>();
        }

        public void OnUpdate(ref SystemState state) {
            foreach (var (healthBarParts, team)
                in SystemAPI.Query<HealthBarParts, UnitTeam>()
                    .WithAll<HealthBarInitializeTag>()) {

                float4 teamColor = TeamColorsForModels.GetColorByTeam(team.Value);

                state.EntityManager.SetComponentData(
                    healthBarParts.Fill,
                    new URPMaterialPropertyBaseColor() { Value = teamColor });

                state.EntityManager.SetComponentData(
                    healthBarParts.Outline,
                    new URPMaterialPropertyBaseColor() { Value = teamColor });
            }

            EntityQuery healthBars = SystemAPI.QueryBuilder().WithAll<HealthBarInitializeTag>().Build();
            state.EntityManager.RemoveComponent<HealthBarInitializeTag>(healthBars);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(HealthBarClientSystemGroup))]
    [UpdateAfter(typeof(HealthBarInitializationSystem))]
    public partial struct HealthBarUpdateSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            foreach (var (referenceToFillArea, currentHealth, maxHealth)
                in SystemAPI.Query<ReferenceToHealthBarFillArea, CurrentHealthPoints, MaximalHealthPoints>())
                
                state.EntityManager.SetComponentData(
                    referenceToFillArea.Value,
                    new URPMaterialPropertySmoothness() { Value = currentHealth.Value / maxHealth.Value });
            //It is written smoothness, but actually that property is responsible for fill area inside custom shader
        }
    }
}