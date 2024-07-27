using Unity.Entities;
using Unity.Mathematics;

namespace Assets.CodeBase.Mobs.Spawn
{
    public struct InitializeMobSpawnerTag : IComponentData { }

    public struct WaypointSettings
    {
        public ushort TeamAmount;
        public BlobArray<ushort> RouteAmount;
        public BlobArray<ushort> RouteOffsets;
        public BlobArray<ushort> WaypointAmount;
        public BlobArray<ushort> WaypointOffsets;
        public BlobArray<float3> Waypoints;
    }

    public struct WaypointSettingsReference : IComponentData
    {
        public BlobAssetReference<WaypointSettings> Blob;
    }
}
