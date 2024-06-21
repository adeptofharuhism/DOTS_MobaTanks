using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Vehicles.Turrets
{
    public struct TurretUninitialized : IComponentData { }
    public struct TurretModelInitialized : IComponentData { }

    public struct TurretSlot : IComponentData
    {
        public Entity Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct TurretWeaponPrefab : IComponentData
    {
        public Entity Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct TurretWeapon : IComponentData
    {
        public Entity Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Client)]
    public struct TurretModelPrefab : IComponentData
    {
        public Entity Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Client)]
    public struct TurretModel : IComponentData
    {
        public Entity Value;
    }
}
