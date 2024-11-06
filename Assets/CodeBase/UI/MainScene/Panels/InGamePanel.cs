using Assets.CodeBase.Utility.MVVM;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace Assets.CodeBase.UI.MainScene.Panels
{
	public class InGamePanel : UiPanel
	{
		private readonly List<UiPart> _uiParts = new();

		public InGamePanel(
			VisualTreeAsset inGamePanel,
			IInGameModeViewModel inGameModeViewModel)
			: base(inGamePanel) {

			_uiParts.Add(new ShopPart(_panel, inGameModeViewModel));
			_uiParts.Add(new MoneyDisplayPart(_panel, inGameModeViewModel));
			_uiParts.Add(new InventoryPart(_panel, inGameModeViewModel));
		}

		protected override void InitializeSubParts() {
			foreach (UiPart uiPart in _uiParts)
				uiPart.Initialize();
		}

		protected override void DisposeSubParts() {
			foreach (UiPart uiPart in _uiParts)
				uiPart.Dispose();
		}
	}

	public class InventoryPart : UiPart
	{
		private VisualElement _inventory;

		private readonly List<InventoryButton> _inventorySlots = new();
		private readonly IInventoryViewModel _inventoryViewModel;

		public InventoryPart(VisualElement parent, IInventoryViewModel inventoryViewModel)
			: base(parent) {

			_inventoryViewModel = inventoryViewModel;
		}

		protected override void CacheVisualElements() {
			_inventory = _parent.Q<VisualElement>(Constants.VisualElementNames.GameUI.InGamePanel.Inventory);
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
				InventoryButton button = new();

				_inventorySlots.Add(button);
				_inventory.Add(button.VisualElement);
			}
		}

		private void ChangeItem(int slotId, int itemId, Texture2D image) {
			_inventorySlots[slotId].ChangeItem(itemId, image);
		}

		private void OnInventoryButtonClicked(int itemId) {
			
		}
	}

	public class ShopPart : UiPart
	{
		private VisualElement _itemGroupContainer;

		private readonly IShopViewModel _shopViewModel;

		public ShopPart(VisualElement parent, IShopViewModel shopViewModel)
			: base(parent) {

			_shopViewModel = shopViewModel;
		}

		protected override void CacheVisualElements() {
			_itemGroupContainer =
				_parent.Q<VisualElement>(Constants.VisualElementNames.GameUI.InGamePanel.ItemGroupContainer);

			InventoryButton button = new();
			button.Button.RegisterCallback<ClickEvent>(_=>_shopViewModel.BuyItem(0));
			_itemGroupContainer.Add(button.VisualElement);
		}

		protected override void ReadInitialViewModelData() {
			UpdateItemGroupContainerVisibility(_shopViewModel.ShopIsVisible.Value);
		}

		protected override void BindData() {
			_shopViewModel.ShopIsVisible.OnChanged += UpdateItemGroupContainerVisibility;
		}

		protected override void RegisterCallbacks() { }

		protected override void UnregisterCallbacks() { }

		protected override void UnbindData() {
			_shopViewModel.ShopIsVisible.OnChanged -= UpdateItemGroupContainerVisibility;
		}

		private void UpdateItemGroupContainerVisibility(bool isVisible) {
			_itemGroupContainer.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
			_itemGroupContainer.SetEnabled(isVisible);
		}
	}

	public class MoneyDisplayPart : UiPart
	{
		private Label _moneyLabel;
		private Button _shopButton;

		private readonly IMoneyDisplayViewModel _moneyDisplayViewModel;

		public MoneyDisplayPart(VisualElement parent, IMoneyDisplayViewModel moneyDisplayViewModel)
			: base(parent) {

			_moneyDisplayViewModel = moneyDisplayViewModel;
		}

		protected override void CacheVisualElements() {
			_moneyLabel = _parent.Q<Label>(Constants.VisualElementNames.GameUI.InGamePanel.MoneyLabel);
			_shopButton = _parent.Q<Button>(Constants.VisualElementNames.GameUI.InGamePanel.ShopButton);
		}

		protected override void ReadInitialViewModelData() {
			UpdateMoneyValue(_moneyDisplayViewModel.MoneyTextView.Value);
		}

		protected override void BindData() {
			_moneyDisplayViewModel.MoneyTextView.OnChanged += UpdateMoneyValue;

		}

		protected override void RegisterCallbacks() {
			_shopButton.RegisterCallback<ClickEvent>(OnClickShop);
		}

		protected override void UnregisterCallbacks() {
			_shopButton.UnregisterCallback<ClickEvent>(OnClickShop);
		}

		protected override void UnbindData() {
			_moneyDisplayViewModel.MoneyTextView.OnChanged -= UpdateMoneyValue;
		}

		private void UpdateMoneyValue(string moneyText) =>
			_moneyLabel.text = moneyText;

		private void OnClickShop(ClickEvent evt) {
			_moneyDisplayViewModel.ClickShop();
		}
	}
}