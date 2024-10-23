using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Inventory
{
    //Settings
    public struct BasicInventoryCapacity : IComponentData
    {
        public int Value;
    }

    //Inventory components
    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct InventoryInitializationTag : IComponentData { }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct InventoryTag : IComponentData { }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct ItemSlotCollection : ICleanupComponentData
    {
        public NativeArray<InventorySlot> Slots;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct InventorySlotEntity : IComponentData
    {
        public Entity Value;
    }

    public struct InventorySlot
    {
        public int ItemId;
        public Entity SpawnedItem;
    }

    //Rpcs
    public struct BuyItemRpc : IRpcCommand
    {
        public int ItemId;
    }

    public struct RemoveItemFromInventoryRpc : IRpcCommand
    {
        public int Slot;
    }

    public struct SwapSlotsInsideInventoryRpc : IRpcCommand
    {
        public int FromSlot;
        public int ToSlot;
    }
}
