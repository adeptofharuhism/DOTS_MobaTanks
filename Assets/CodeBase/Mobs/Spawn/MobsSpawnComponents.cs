using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace Assets.CodeBase.Mobs.Spawn
{
    public struct InitializeSpawnRequestProcessTag : IComponentData { }

    public struct WaypointSettings
    {
        public ushort TeamAmount;
        public BlobArray<ushort> RouteAmount;
        public BlobArray<ushort> RouteOffsets;
        public BlobArray<ushort> WaypointAmount;
        public BlobArray<ushort> WaypointOffsets;
        public BlobArray<float3> Waypoints;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct WaypointSettingsReference : IComponentData
    {
        public BlobAssetReference<WaypointSettings> Blob;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct NewSpawnRequestElement : IBufferElementData
    {
        public ushort Team;
        public ushort WaveCooldown;
        public Entity MobPrefab;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct NewSpawnerInstantiationParametersElement : IBufferElementData
    {
        public Entity SpawnerPrefab;
        public Entity MobPrefab;
        public ushort WaveCooldown;
        public ushort CurrentRoute;
        public ushort RouteAmount;
        public ushort Team;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct SpawnerPrefab : IComponentData
    {
        public Entity Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct MobPrefab : IComponentData
    {
        public Entity Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct RouteInformation : IComponentData
    {
        public ushort RouteAmount;
        public ushort CurrentRoute;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct MobSpawnCooldown : IComponentData
    {
        public float Cooldown;
        public float CurrentTimeLeft;
    }
}
