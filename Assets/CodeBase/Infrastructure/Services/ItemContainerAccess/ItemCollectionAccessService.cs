using Assets.CodeBase.Inventory.Items;

namespace Assets.CodeBase.Infrastructure.Services.ItemCollectionAccess
{
    public class ItemCollectionAccessService : IItemContainerAccess
    {
        private readonly ItemCollection _itemCollection;

        public ItemCollectionAccessService(ItemCollection itemCollection) {
            _itemCollection = itemCollection;
        }
    }
}
