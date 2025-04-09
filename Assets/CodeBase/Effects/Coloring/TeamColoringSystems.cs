using Assets.CodeBase.Teams;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace Assets.CodeBase.Effects.Coloring
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(TeamColoringSystemGroup))]
    public partial struct InitialTeamColoringClientSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<TeamColorElement>();
            state.RequireForUpdate<InitialTeamColoringTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new(Allocator.Temp);
            DynamicBuffer<TeamColorElement> colors = SystemAPI.GetSingletonBuffer<TeamColorElement>();

            foreach (var (team, entitiesWithRenderer, entity)
                in SystemAPI.Query<UnitTeam, DynamicBuffer<EntitiesWithRendererElement>>()
                    .WithAll<InitialTeamColoringTag>()
                    .WithEntityAccess()) {

                float4 teamColor =
                    colors[(int)team.Value]
                        .Value;

                foreach (EntitiesWithRendererElement entityWithRenderer in entitiesWithRenderer)
                    ecb.AddComponent(entityWithRenderer.Value, new URPMaterialPropertyBaseColor { Value = teamColor });

                ecb.RemoveComponent<InitialTeamColoringTag>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}