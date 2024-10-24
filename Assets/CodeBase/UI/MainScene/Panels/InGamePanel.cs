using Assets.CodeBase.Utility.MVVM;
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

        protected override void OnConstruction() {
            _leftPart = _panel.Q<VisualElement>(Constants.VisualElementNames.GameUI.InGamePanel.LeftPart);
            _centralPart = _panel.Q<VisualElement>(Constants.VisualElementNames.GameUI.InGamePanel.CentralPart);
            _rightPart = _panel.Q<VisualElement>(Constants.VisualElementNames.GameUI.InGamePanel.RightPart);
        }

        public override void Enable() {
            AddSubPanels();
            InitializeSubPanels();
            EnableSubPanels();
        }

        public override void Disable() {
            DisableSubPanels();
            DisposeSubPanels();
            RemoveSubPanels();
        }

        private void AddSubPanels() {
            _rightPart.AddUiPanel(_shopPanel);
        }

        private void EnableSubPanels() {
            _shopPanel.Enable();
        }

        private void InitializeSubPanels() {
            _shopPanel.Initialize();
        }

        private void DisposeSubPanels() {
            _shopPanel.Dispose();
        }

        private void DisableSubPanels() {
            _shopPanel.Disable();
        }

        private void RemoveSubPanels() {
            _rightPart.RemoveUiPanel(_shopPanel);
        }
    }

    public class ShopPanel : UiPanel
    {
        private VisualElement _shopPart;
        private Button _shopButton;
        private Label _moneyLabel;

        private bool _shopIsShown = false;
        private bool _shopCanBeShown;

        private readonly IShopActivationViewModel _shopViewModel;
        private readonly AvailableItemsPanel _availableItemsPanel;

        public ShopPanel(VisualTreeAsset panelAsset, IInGameModeViewModel inGameModeViewModel, VisualTreeAsset availableItemsPanel)
            : base(panelAsset) {

            _shopViewModel = inGameModeViewModel;

            _availableItemsPanel = new AvailableItemsPanel(availableItemsPanel, inGameModeViewModel);
        }

        protected override void OnConstruction() {
            _shopPart = _panel.Q<VisualElement>(Constants.VisualElementNames.GameUI.InGamePanel.ShopPanel.ShopPart);
            _shopButton = _panel.Q<Button>(Constants.VisualElementNames.GameUI.InGamePanel.ShopPanel.ShopButton);
            _moneyLabel = _panel.Q<Label>(Constants.VisualElementNames.GameUI.InGamePanel.ShopPanel.MoneyLabel);
        }

        public override void Enable() {
            _shopButton.RegisterCallback<ClickEvent>(OnClickShop);

            EnableSubPanel();
        }

        public override void Disable() {
            _shopButton.UnregisterCallback<ClickEvent>(OnClickShop);

            DisableSubPanel();
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

        private void EnableSubPanel() {
            _availableItemsPanel.Initialize();
            _availableItemsPanel.Enable();
        }

        private void DisableSubPanel() {
            _availableItemsPanel.Disable();
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
        private readonly IItemRequestViewModel _itemRequestViewModel;

        private Button _testButton;

        public AvailableItemsPanel(VisualTreeAsset panelAsset, IItemRequestViewModel itemRequestViewModel)
            : base(panelAsset) {

            _itemRequestViewModel = itemRequestViewModel;
        }

        protected override void OnConstruction() {
            _testButton = _panel.Q<Button>("Test");
        }

        public override void Enable() {
            _testButton.RegisterCallback<ClickEvent>(OnClickTest);
        }

        public override void Disable() {
            _testButton.UnregisterCallback<ClickEvent>(OnClickTest);
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

        private void UpdateAvailableItems(int money) {

        }
    }
}
