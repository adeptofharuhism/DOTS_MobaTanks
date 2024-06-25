using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace Assets.CodeBase.Combat.Teams.Coloring
{
    [UpdateInGroup(typeof(TransformSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
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
