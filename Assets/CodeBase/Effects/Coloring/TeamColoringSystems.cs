using Assets.CodeBase.Teams;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace Assets.CodeBase.Effects.Coloring
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(TeamColoringSystemGroup))]
    public partial struct InitialTeamColoringClientSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (team, entitiesWithRenderer, entity)
                in SystemAPI.Query<UnitTeam, DynamicBuffer<EntitiesWithRendererElement>>()
                .WithAll<InitialTeamColoringTag>()
                .WithEntityAccess()) {

                float4 teamColor = TeamColorsForModels.GetColorByTeam(team.Value);
                foreach (EntitiesWithRendererElement entityWithRenderer in entitiesWithRenderer)
                    ecb.AddComponent(entityWithRenderer.Value, new URPMaterialPropertyBaseColor { Value = teamColor });

                ecb.RemoveComponent<InitialTeamColoringTag>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
