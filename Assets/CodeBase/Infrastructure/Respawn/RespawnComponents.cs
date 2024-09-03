using Assets.CodeBase.Teams;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.CodeBase.Infrastructure.Respawn
{
    public struct VehicleRespawnParameters : IComponentData
    {
        public int ClientId;
        public TeamType Team;
        public FixedString64Bytes PlayerName;
        public float3 SpawnPosition;
        public Entity VehiclePrefab;
    }

    public struct ShouldRespawnTag : IComponentData { }
    public struct OnRespawnCooldownTag : IComponentData { }
    public struct RespawnedEntityIsAliveTag : IComponentData { }

    public struct RespawnCooldown : IComponentData
    {
        public float Value;
    }

    public struct TimeToRespawn : IComponentData
    {
        public float Value;
    }

    public struct RespawnedEntity : ICleanupComponentData
    {
        public Entity Value;
    }
}
