using Assets.CodeBase.Utility.MVVM;
using System;
using UnityEngine.UIElements;

namespace Assets.CodeBase.UI.MainScene.Panels
{
	public class InGamePanel : UiPanel
	{
		private readonly MoneyDisplayPart _moneyDisplayPart;
		private readonly ShopPart _shopPart;

		public InGamePanel(
			VisualTreeAsset inGamePanel,
			IInGameModeViewModel inGameModeViewModel)
			: base(inGamePanel) {

			_moneyDisplayPart = new MoneyDisplayPart(_panel, inGameModeViewModel);
			_shopPart = new ShopPart(_panel, inGameModeViewModel);
		}

		protected override void InitializeSubParts() {
			_moneyDisplayPart.Initialize();
			_shopPart.Initialize();
		}

		protected override void DisposeSubParts() {
			_moneyDisplayPart.Dispose();
			_shopPart.Dispose();
		}
	}

	public class ShopPart
	{
		private VisualElement _itemGroupContainer;

		private readonly VisualElement _parent;
		private readonly IShopViewModel _shopViewModel;

		public ShopPart(VisualElement parent, IShopViewModel shopViewModel) {
			_parent = parent;
			_shopViewModel = shopViewModel;
		}

		public void Initialize() {
			CacheVisualElements();
			ReadInitialViewModelData();
			BindData();
			RegisterCallbacks();
		}

		public void Dispose() {
			UnregisterCallbacks();
			UnbindData();
		}

		private void CacheVisualElements() {
			_itemGroupContainer =
				_parent.Q<VisualElement>(Constants.VisualElementNames.GameUI.InGamePanel.ItemGroupContainer);
		}

		private void ReadInitialViewModelData() {
			UpdateItemGroupContainerVisibility(_shopViewModel.ShopIsVisible.Value);
		}

		private void BindData() {
			_shopViewModel.ShopIsVisible.OnChanged += UpdateItemGroupContainerVisibility;
		}

		private void RegisterCallbacks() { }

		private void UnregisterCallbacks() { }

		private void UnbindData() {
			_shopViewModel.ShopIsVisible.OnChanged -= UpdateItemGroupContainerVisibility;
		}

		private void UpdateItemGroupContainerVisibility(bool isVisible) {
			_itemGroupContainer.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
			_itemGroupContainer.SetEnabled(isVisible);
		}
	}

	public class MoneyDisplayPart
	{
		private Label _moneyLabel;
		private Button _shopButton;

		private readonly VisualElement _parent;
		private readonly IMoneyDisplayViewModel _moneyDisplayViewModel;

		public MoneyDisplayPart(VisualElement parent, IMoneyDisplayViewModel moneyDisplayViewModel) {
			_parent = parent;
			_moneyDisplayViewModel = moneyDisplayViewModel;
		}

		public void Initialize() {
			CacheVisualElements();
			ReadInitialViewModelData();
			BindData();
			RegisterCallbacks();
		}

		public void Dispose() {
			UnregisterCallbacks();
			UnbindData();
		}

		private void CacheVisualElements() {
			_moneyLabel = _parent.Q<Label>(Constants.VisualElementNames.GameUI.InGamePanel.MoneyLabel);
			_shopButton = _parent.Q<Button>(Constants.VisualElementNames.GameUI.InGamePanel.ShopButton);
		}

		private void ReadInitialViewModelData() {
			UpdateMoneyValue(_moneyDisplayViewModel.MoneyTextView.Value);
		}

		private void BindData() {
			_moneyDisplayViewModel.MoneyTextView.OnChanged += UpdateMoneyValue;

		}

		private void RegisterCallbacks() {
			_shopButton.RegisterCallback<ClickEvent>(OnClickShop);
		}

		private void UnregisterCallbacks() {
			_shopButton.UnregisterCallback<ClickEvent>(OnClickShop);
		}

		private void UnbindData() {
			_moneyDisplayViewModel.MoneyTextView.OnChanged -= UpdateMoneyValue;
		}

		private void UpdateMoneyValue(string moneyText) =>
			_moneyLabel.text = moneyText;

		private void OnClickShop(ClickEvent evt) {
			_moneyDisplayViewModel.OnClickShop();
		}
	}
}