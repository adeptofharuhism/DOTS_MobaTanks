using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Targeting
{
    [GhostComponent(PrefabType =GhostPrefabType.Server)]
    public partial struct Targetable : IComponentData { }

    [GhostComponent(PrefabType =GhostPrefabType.Server)]
    public partial struct TargetPoint : IComponentData
    {
        public Entity Value;
    }

    [GhostComponent(PrefabType =GhostPrefabType.Server)]
    public partial struct Targeter : IComponentData, IEnableableComponent { }

    [GhostComponent(PrefabType =GhostPrefabType.Server)]
    public partial struct TargetOnCertainPointTag : IComponentData { }

    public enum TargetingType
    {
        Closest,
        Random
    }

    public partial struct TargeterRange : IComponentData
    {
        public float Value;
    }

    [GhostComponent(PrefabType =GhostPrefabType.Server)]
    public partial struct CurrentTarget : IComponentData
    {
        public Entity Value;
    }
}
