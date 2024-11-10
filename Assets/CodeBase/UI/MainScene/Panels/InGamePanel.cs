using Assets.CodeBase.Constants;
using Assets.CodeBase.Infrastructure.Services.UiFactories;
using Assets.CodeBase.Utility.MVVM;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace Assets.CodeBase.UI.MainScene.Panels
{
	public class InGamePanel : UiPanel
	{
		private readonly List<UiPart> _uiParts = new();

		public InGamePanel(
			VisualTreeAsset inGamePanel,
			IInGameModeViewModel inGameModeViewModel,
			IInventoryButtonFactory inventoryButtonFactory,
			IShopButtonFactory shopButtonFactory)
			: base(inGamePanel) {

			_uiParts.Add(new ShopPart(_panel, inGameModeViewModel, shopButtonFactory));
			_uiParts.Add(new MoneyDisplayPart(_panel, inGameModeViewModel));
			_uiParts.Add(new InventoryPart(_panel, inGameModeViewModel, inventoryButtonFactory));
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
			Debug.Log($"Clicked on {slotId} slot");
		}
	}

	public class ShopPart : UiPart
	{
		private VisualElement _itemGroupContainer;
		private ShopButton[] _sortedShopButtons;
		private int _leastExpensiveDisabledButtonIndex;
		private int _lastMoneyUpdate;

		private readonly IShopViewModel _shopViewModel;
		private readonly IShopButtonFactory _shopButtonFactory;

		public ShopPart(VisualElement parent, IShopViewModel shopViewModel, IShopButtonFactory shopButtonFactory)
			: base(parent) {

			_shopViewModel = shopViewModel;
			_shopButtonFactory = shopButtonFactory;
		}

		protected override void CacheVisualElements() {
			_itemGroupContainer =
				_parent.Q<VisualElement>(VisualElementNames.GameUI.InGamePanel.ItemGroupContainer);

			VisualElement[] itemGroups = _shopButtonFactory.CreateItemGroups(OnClickShopButton, out _sortedShopButtons);

			foreach (VisualElement itemGroup in itemGroups)
				_itemGroupContainer.Add(itemGroup);
		}

		protected override void ReadInitialViewModelData() {
			UpdateItemGroupContainerVisibility(_shopViewModel.ShopIsVisible.Value);
			UpdateActiveButtons(_shopViewModel.MoneyView.Value);
		}

		protected override void BindData() {
			_shopViewModel.ShopIsVisible.OnChanged += UpdateItemGroupContainerVisibility;
			_shopViewModel.MoneyView.OnChanged += UpdateActiveButtons;
		}

		protected override void RegisterCallbacks() { }

		protected override void UnregisterCallbacks() { }

		protected override void UnbindData() {
			_shopViewModel.ShopIsVisible.OnChanged -= UpdateItemGroupContainerVisibility;
			_shopViewModel.MoneyView.OnChanged -= UpdateActiveButtons;
		}

		private void UpdateItemGroupContainerVisibility(bool isVisible) {
			_itemGroupContainer.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
			_itemGroupContainer.SetEnabled(isVisible);
		}

		private void OnClickShopButton(int itemId) =>
			_shopViewModel.BuyItem(itemId);

		private void UpdateActiveButtons(int money) {
			if (money > _lastMoneyUpdate)
				ActivateAffordableButtons(money);
			else
				DeactivateExpensiveButtons(money);

			_lastMoneyUpdate = money;
		}

		private void ActivateAffordableButtons(int money) {
			while (_leastExpensiveDisabledButtonIndex<_sortedShopButtons.Length) {
				if (money < _sortedShopButtons[_leastExpensiveDisabledButtonIndex].ItemCost)
					return;
				
				_sortedShopButtons[_leastExpensiveDisabledButtonIndex++].Enable();
			}
		}

		private void DeactivateExpensiveButtons(int money) {
			while (_leastExpensiveDisabledButtonIndex>0) {
				if (money >= _sortedShopButtons[_leastExpensiveDisabledButtonIndex-1].ItemCost)
					return;
				
				_sortedShopButtons[_leastExpensiveDisabledButtonIndex-1].Disable();
				_leastExpensiveDisabledButtonIndex--;
			}
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
			_moneyLabel = _parent.Q<Label>(VisualElementNames.GameUI.InGamePanel.MoneyLabel);
			_shopButton = _parent.Q<Button>(VisualElementNames.GameUI.InGamePanel.ShopButton);
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