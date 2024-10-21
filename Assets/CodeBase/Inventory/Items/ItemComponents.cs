using Unity.Entities;

namespace Assets.CodeBase.Inventory.Items
{
    public struct Item : IBufferElementData
    {
        public Entity Weapon;
    }
}
