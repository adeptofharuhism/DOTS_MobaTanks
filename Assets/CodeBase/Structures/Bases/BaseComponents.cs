using Assets.CodeBase.Teams;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace Assets.CodeBase.Structures.Bases
{
    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct BaseSpawnPositions : IComponentData
    {
        public float3 BlueBase;
        public float3 OrangeBase;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct BaseTag : IComponentData { }
    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct BaseTeamCleanUp : ICleanupComponentData
    {
        public TeamType Team;
    }
}
