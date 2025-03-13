using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Inventory.TargetingRange
{
    [GhostComponent(PrefabType = GhostPrefabType.Client)]
    public struct ItemInfoElement : IBufferElementData
    {
        public float TargetRange;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Client)]
    public struct UpdateTargetRangeElement : IBufferElementData
    {
        public int SlotId;
        public float TargetRange;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Client)]
    public struct TargetingDecalPrefabs : IComponentData
    {
        public Entity CloseRangeDecalPrefab;
        public Entity FarRangeDecalPrefab;
    }
}
