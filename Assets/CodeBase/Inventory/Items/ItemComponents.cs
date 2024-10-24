using Assets.CodeBase.Teams;
using Unity.Entities;

namespace Assets.CodeBase.Inventory.Items
{
    public struct ItemCommandTag : IComponentData { }
    public struct ItemCreationTag : IComponentData { }
    public struct ItemRemovalTag : IComponentData { }

    public struct ItemCreationPrefab : IBufferElementData
    {
        public int BuyCost;
        public Entity Command;
    }

    public struct ItemRemovalPrefab : IBufferElementData
    {
        public int SellCost;
        public Entity Item;
    }

    public struct SpawnableItem : IComponentData
    {
        public Entity Value;
    }

    public struct SpawnableItemSettings : IComponentData
    {
        public int InventorySlot;
        public TeamType ItemTeam;
        public Entity PlayerEntity;
        public Entity SpawnParent;
        public Entity Vehicle;
    }
}
