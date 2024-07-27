using Unity.Entities;

namespace Assets.CodeBase.Mobs.Spawn
{
    public struct InitializeMobSpawnerTag : IComponentData { }

    public struct TeamWaypointSets : IComponentData
    {
        public BlobAssetReference<float> Value;
    }
}
