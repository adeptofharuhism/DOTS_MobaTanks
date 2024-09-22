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
    public struct ItemEntityCollection : ICleanupComponentData
    {
        public NativeArray<Entity> Items;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct InventorySlotEntity : IComponentData
    {
        public Entity Value;
    }

    //Rpcs
    public struct AddItemToInventoryRpc : IRpcCommand
    {
        
    }
}
