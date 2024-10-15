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

        protected override void CacheVisualElements() {
            _leftPart = _panel.Q<VisualElement>(Constants.VisualElementNames.GameUI.InGamePanel.LeftPart);
            _centralPart = _panel.Q<VisualElement>(Constants.VisualElementNames.GameUI.InGamePanel.CentralPart);
            _rightPart = _panel.Q<VisualElement>(Constants.VisualElementNames.GameUI.InGamePanel.RightPart);
        }

        public override void Enable() {
            AddSubPanels();
            EnableSubPanels();
        }

        public override void Disable() {
            DisableSubPanels();
            RemoveSubPanels();
        }

        private void AddSubPanels() {
            _rightPart.AddUiPanel(_shopPanel);
        }

        private void EnableSubPanels() {
            _shopPanel.Enable();
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
        private bool _shopCanBeShown = false;

        private readonly IShopViewModel _shopViewModel;
        private readonly AvailableItemsPanel _availableItemsPanel;

        public ShopPanel(VisualTreeAsset panelAsset, IInGameModeViewModel inGameModeViewModel, VisualTreeAsset availableItemsPanel)
            : base(panelAsset) {

            _shopViewModel = inGameModeViewModel;

            _availableItemsPanel = new AvailableItemsPanel(availableItemsPanel);
        }

        protected override void CacheVisualElements() {
            _shopPart = _panel.Q<VisualElement>(Constants.VisualElementNames.GameUI.InGamePanel.ShopPanel.ShopPart);
            _shopButton = _panel.Q<Button>(Constants.VisualElementNames.GameUI.InGamePanel.ShopPanel.ShopButton);
            _moneyLabel = _panel.Q<Label>(Constants.VisualElementNames.GameUI.InGamePanel.ShopPanel.MoneyLabel);
        }

        public override void Enable() {
            _shopButton.RegisterCallback<ClickEvent>(OnClickShop);
        }

        public override void Disable() {
            _shopButton.UnregisterCallback<ClickEvent>(OnClickShop);
        }

        protected override void BindData() {
            _shopViewModel.MoneyView.OnChanged += OnChangedMoney;
            _shopViewModel.ShopCanBeShown.OnChanged += OnChangedShopAvailabilityFlag;
        }

        protected override void UnbindData() {
            _shopViewModel.MoneyView.OnChanged -= OnChangedMoney;
        }

        private void OnChangedMoney(string money) =>
            _moneyLabel.text = money;

        private void OnChangedShopAvailabilityFlag(bool flag) {
            _shopCanBeShown = flag;

            if (!_shopCanBeShown)
                HideItemsPanel();
        }

        private void OnClickShop(ClickEvent evt) {
            if (_shopIsShown)
                ShowItemsPanel();
            else
                HideItemsPanel();
        }

        private void ShowItemsPanel() {
            if (!_shopCanBeShown)
                return;

            _shopIsShown = true;
            _shopPart.AddUiPanel(_availableItemsPanel);
        }

        private void HideItemsPanel() {
            _shopIsShown = false;
            _shopPart.RemoveUiPanel(_availableItemsPanel);
        }
    }

    public class AvailableItemsPanel : UiPanel
    {
        public AvailableItemsPanel(VisualTreeAsset panelAsset)
            : base(panelAsset) {

        }
    }
}
