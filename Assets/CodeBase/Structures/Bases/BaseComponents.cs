using Assets.CodeBase.Combat.Teams;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.CodeBase.Structures.Bases
{
    public struct BaseSpawnPositions : IComponentData
    {
        public float3 BlueBase;
        public float3 OrangeBase;
    }

    public struct BaseTag : IComponentData { }
    public struct BaseTeamCleanUp : ICleanupComponentData
    {
        public TeamType Team;
    }
}
