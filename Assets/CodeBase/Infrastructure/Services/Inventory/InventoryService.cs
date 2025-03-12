using Assets.CodeBase.Infrastructure.Services.WorldAccess;
using Assets.CodeBase.Inventory;
using Assets.CodeBase.Utility;
using System;
using Zenject;

namespace Assets.CodeBase.Infrastructure.Services.Inventory
{
    public class InventoryService : IInventoryService, IInitializable
    {
        public event Action<int, int> OnChangedItem;

        public IReactiveGetter<int> InventorySize => _inventorySize;

        private readonly ReactiveProperty<int> _inventorySize = new();

        private readonly IWorldAccessService _worldAccessService;

        public InventoryService(IWorldAccessService worldAccessService) {
            _worldAccessService = worldAccessService;
        }

        public void Initialize() {
            SubscribeToInventoryChanges();
        }

        private void SubscribeToInventoryChanges() {
            UpdateClientInventorySystem inventorySystem =
                _worldAccessService.DefaultWorld.GetExistingSystemManaged<UpdateClientInventorySystem>();


            inventorySystem.InventorySize.OnChanged += UpdateInventorySize;
            inventorySystem.OnChangedItem += UpdateItem;
        }

        private void UpdateInventorySize(int size) =>
            _inventorySize.Value = size;

        private void UpdateItem(int slotIndex, int itemId) =>
            OnChangedItem?.Invoke(slotIndex, itemId);
    }
}