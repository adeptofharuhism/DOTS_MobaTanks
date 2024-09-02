using Assets.CodeBase.Teams;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace Assets.CodeBase.Effects.Coloring
{
    public static class TeamColorsForModels
    {
        private static Dictionary<TeamType, float4> _colors =
            new Dictionary<TeamType, float4> {
                [TeamType.None] = new float4(1, 1, 1, 1),
                [TeamType.Blue] = new float4(0, 1, 1, 1),
                [TeamType.Orange] = new float4(1, .4f, 0, 1)
            };

        public static float4 GetColorByTeam(TeamType team) => _colors[team];
    }

    [GhostComponent(PrefabType = GhostPrefabType.Client)]
    public struct InitialTeamColoringTag : IComponentData { }

    [GhostComponent(PrefabType = GhostPrefabType.Client)]
    public struct EntitiesWithRendererElement : IBufferElementData
    {
        public Entity Value;
    }
}
