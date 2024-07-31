using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEditor;

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
        public ushort RouteOffset;
        public ushort Team;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct SpawnerPrefab : IComponentData
    {
        public Entity Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct ShouldSpawnMobTag : IComponentData { }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct MobPrefab : IComponentData
    {
        public Entity Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct CurrentRoute : IComponentData
    {
        public ushort Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct RouteAmount : IComponentData
    {
        public ushort Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct RouteOffset : IComponentData
    {
        public ushort Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct MobSpawnCooldown : IComponentData
    {
        public float Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct MobSpawnCooldownTimeLeft : IComponentData
    {
        public float Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct MobSpawnPosition : IComponentData
    {
        public float3 Value;
    }
}
