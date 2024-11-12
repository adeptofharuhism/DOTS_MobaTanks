using Assets.CodeBase.Constants;
using Assets.CodeBase.Infrastructure.Services.UiFactories;
using Assets.CodeBase.Utility.MVVM;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.CodeBase.UI.MainScene.Panels
{
	public class InventoryPart : UiPart
	{
		private VisualElement _inventory;

		private readonly List<InventoryButton> _inventorySlots = new();
		private readonly IInventoryViewModel _inventoryViewModel;
		private readonly IInventoryButtonFactory _inventoryButtonFactory;

		public InventoryPart(
			VisualElement parent,
			IInventoryViewModel inventoryViewModel,
			IInventoryButtonFactory inventoryButtonFactory)
			: base(parent) {

			_inventoryViewModel = inventoryViewModel;
			_inventoryButtonFactory = inventoryButtonFactory;
		}

		protected override void CacheVisualElements() {
			_inventory = _parent.Q<VisualElement>(VisualElementNames.GameUI.InGamePanel.Inventory);
		}

		protected override void BindData() {
			_inventoryViewModel.InventorySizeView.OnChanged += CreateInventorySlots;
			_inventoryViewModel.OnChangedItem += ChangeItem;
		}

		protected override void UnbindData() {
			_inventoryViewModel.InventorySizeView.OnChanged -= CreateInventorySlots;
			_inventoryViewModel.OnChangedItem -= ChangeItem;
		}

		private void CreateInventorySlots(int size) {
			for (int i = 0; i < size; i++) {
				InventoryButton button = _inventoryButtonFactory.CreateButton(OnClickInventoryButton, i);

				_inventorySlots.Add(button);
				_inventory.Add(button.VisualElement);
			}
		}

		private void ChangeItem(int slotId, int itemId) {
			_inventorySlots[slotId].ItemId = itemId;
		}

		private void OnClickInventoryButton(int slotId) {
			_inventoryViewModel.SellItem(slotId);
		}
	}
}