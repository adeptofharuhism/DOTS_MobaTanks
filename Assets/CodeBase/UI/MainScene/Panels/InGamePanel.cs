using Assets.CodeBase.Utility.MVVM;
using System;
using UnityEngine.UIElements;

namespace Assets.CodeBase.UI.MainScene.Panels
{
	public class InGamePanel : UiPanel
	{
		private readonly ShopPanel _shopPanel;

		private VisualElement _leftPart;
		private VisualElement _centralPart;
		private VisualElement _rightPart;

		public InGamePanel(
			VisualTreeAsset inGamePanel,
			IInGameModeViewModel inGameModeViewModel,
			VisualTreeAsset shopPanel,
			VisualTreeAsset availableItemsPanel)
			: base(inGamePanel) {

			_shopPanel = new ShopPanel(shopPanel, inGameModeViewModel, availableItemsPanel);
		}

		protected override void CacheVisualElements() {
			_leftPart = _panel.Q<VisualElement>(Constants.VisualElementNames.GameUI.InGamePanel.LeftPart);
			_centralPart = _panel.Q<VisualElement>(Constants.VisualElementNames.GameUI.InGamePanel.CentralPart);
			_rightPart = _panel.Q<VisualElement>(Constants.VisualElementNames.GameUI.InGamePanel.RightPart);
		}

		protected override void InitializeSubPanels() {
			_rightPart.AddUiPanel(_shopPanel);
			_shopPanel.Initialize();
		}

		protected override void DisposeSubPanels() {
			_shopPanel.Dispose();
			_rightPart.RemoveUiPanel(_shopPanel);
		}
	}

	public class ShopPanel : UiPanel
	{
		private VisualElement _shopPart;
		private Button _shopButton;
		private Label _moneyLabel;

		private bool _shopIsShown;
		private bool _shopCanBeShown;

		private readonly IShopActivationViewModel _shopViewModel;
		private readonly AvailableItemsPanel _availableItemsPanel;

		public ShopPanel(VisualTreeAsset panelAsset, IInGameModeViewModel inGameModeViewModel,
			VisualTreeAsset availableItemsPanel)
			: base(panelAsset) {

			_shopViewModel = inGameModeViewModel;

			_availableItemsPanel = new AvailableItemsPanel(availableItemsPanel, inGameModeViewModel);
		}

		protected override void CacheVisualElements() {
			_shopPart = _panel.Q<VisualElement>(Constants.VisualElementNames.GameUI.InGamePanel.ShopPanel.ShopPart);
			_shopButton = _panel.Q<Button>(Constants.VisualElementNames.GameUI.InGamePanel.ShopPanel.ShopButton);
			_moneyLabel = _panel.Q<Label>(Constants.VisualElementNames.GameUI.InGamePanel.ShopPanel.MoneyLabel);
		}

		protected override void RegisterCallbacks() {
			_shopButton.RegisterCallback<ClickEvent>(OnClickShop);
		}

		protected override void UnregisterCallbacks() {
			_shopButton.UnregisterCallback<ClickEvent>(OnClickShop);
		}

		protected override void ReadInitialViewModelData() {
			ChangeMoneyValue(_shopViewModel.MoneyTextView.Value);
			ChangeAvailabilityFlag(_shopViewModel.ShopCanBeShown.Value);
		}

		protected override void BindData() {
			_shopViewModel.MoneyTextView.OnChanged += ChangeMoneyValue;
			_shopViewModel.ShopCanBeShown.OnChanged += ChangeAvailabilityFlag;
		}

		protected override void UnbindData() {
			_shopViewModel.MoneyTextView.OnChanged -= ChangeMoneyValue;
			_shopViewModel.ShopCanBeShown.OnChanged -= ChangeAvailabilityFlag;
		}

		protected override void InitializeSubPanels() {
			_availableItemsPanel.Initialize();
		}

		protected override void DisposeSubPanels() {
			_availableItemsPanel.Dispose();
		}

		private void OnClickShop(ClickEvent evt) {
			if (_shopIsShown)
				HideItemsPanel();
			else
				ShowItemsPanel();
		}

		private void ChangeMoneyValue(string money) =>
			_moneyLabel.text = money;

		private void ChangeAvailabilityFlag(bool flag) {
			_shopCanBeShown = flag;

			if (!_shopCanBeShown)
				HideItemsPanel();
		}

		private void ShowItemsPanel() {
			if (!_shopCanBeShown)
				return;


			_shopIsShown = true;
			_shopPart.AddUiPanel(_availableItemsPanel);
		}

		private void HideItemsPanel() {
			if (!_shopIsShown)
				return;


			_shopIsShown = false;
			_shopPart.RemoveUiPanel(_availableItemsPanel);
		}
	}

	public class AvailableItemsPanel : UiPanel
	{
		private int RandomFrom0To5 => _random.Next(0, 6);

		private readonly Random _random;
		private readonly IItemRequestViewModel _itemRequestViewModel;

		private Button _testButton;
		private Button _removalButton;
		private Button _swapButton;

		public AvailableItemsPanel(VisualTreeAsset panelAsset, IItemRequestViewModel itemRequestViewModel)
			: base(panelAsset) {

			_random = new Random(42);
			_itemRequestViewModel = itemRequestViewModel;
		}

		protected override void CacheVisualElements() {
			_testButton = _panel.Q<Button>("Test");
			_removalButton = _panel.Q<Button>("Removal");
			_swapButton = _panel.Q<Button>("Swap");
		}

		protected override void RegisterCallbacks() {
			_testButton.RegisterCallback<ClickEvent>(OnClickTest);
			_removalButton.RegisterCallback<ClickEvent>(OnClickRemoval);
			_swapButton.RegisterCallback<ClickEvent>(OnClickSwap);
		}

		protected override void UnregisterCallbacks() {
			_testButton.UnregisterCallback<ClickEvent>(OnClickTest);
			_removalButton.UnregisterCallback<ClickEvent>(OnClickRemoval);
			_swapButton.UnregisterCallback<ClickEvent>(OnClickSwap);
		}

		protected override void ReadInitialViewModelData() {
			UpdateAvailableItems(_itemRequestViewModel.MoneyView.Value);
		}

		protected override void BindData() {
			_itemRequestViewModel.MoneyView.OnChanged += UpdateAvailableItems;
		}

		protected override void UnbindData() {
			_itemRequestViewModel.MoneyView.OnChanged -= UpdateAvailableItems;
		}

		private void OnClickTest(ClickEvent evt) {
			_itemRequestViewModel.BuyItem(0);
		}

		private void OnClickRemoval(ClickEvent evt) {
			_itemRequestViewModel.SellItem(RandomFrom0To5);
		}

		private void OnClickSwap(ClickEvent evt) {
			_itemRequestViewModel.SwapItems(RandomFrom0To5, RandomFrom0To5);
		}

		private void UpdateAvailableItems(int money) { }
	}
}