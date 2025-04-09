using Assets.CodeBase.Utility.Extensions;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.CodeBase.Effects.Coloring
{
    public struct TeamColorElement : IBufferElementData
    {
        public float4 Value;
    }

    public class TeamColorSettingsAuthoring : MonoBehaviour
    {
        [SerializeField] private Color _noneTeamColor;
        [SerializeField] private Color _blueTeamColor;
        [SerializeField] private Color _orangeTeamColor;

        public Color NoneTeamColor => _noneTeamColor;
        public Color BlueTeamColor => _blueTeamColor;
        public Color OrangeTeamColor => _orangeTeamColor;

        private class Baker : Baker<TeamColorSettingsAuthoring>
        {
            public override void Bake(TeamColorSettingsAuthoring authoring) {
                Entity settings = GetEntity(TransformUsageFlags.None);

                DynamicBuffer<TeamColorElement> colorBuffer =  AddBuffer<TeamColorElement>(settings);
                colorBuffer.Add(new TeamColorElement {Value = authoring.NoneTeamColor.AsFloat4()});
                colorBuffer.Add(new TeamColorElement {Value = authoring.BlueTeamColor.AsFloat4()});
                colorBuffer.Add(new TeamColorElement {Value = authoring.OrangeTeamColor.AsFloat4()});
            }
        }
    }
}