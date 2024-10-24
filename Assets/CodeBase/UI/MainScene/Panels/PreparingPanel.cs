using Assets.CodeBase.Utility.MVVM;
using UnityEngine.UIElements;

namespace Assets.CodeBase.UI.MainScene.Panels
{
    public class PreparingPanel : UiPanel
    {
        private AskReadyPanel _askReadyPanel;
        private WaitingPanel _waitingPanel;

        private VisualElement _subContentPanel;

        private readonly INotifyReadyViewModel _notifyReadyViewModel;

        public PreparingPanel(
            VisualTreeAsset preparingPanel,
            IPreparingModeViewModel preparingModeViewModel,
            VisualTreeAsset askReadyPanel,
            VisualTreeAsset waitingPanel)
            : base(preparingPanel) {

            _notifyReadyViewModel = preparingModeViewModel;

            _askReadyPanel = new AskReadyPanel(askReadyPanel, preparingModeViewModel);
            _waitingPanel = new WaitingPanel(waitingPanel);
        }

        protected override void OnConstruction() {
            _subContentPanel =
                _panel.Q<VisualElement>(Constants.VisualElementNames.GameUI.PreparingPanel.SubContentPanel);
        }

        public override void Enable() {
            AddPanelToSubContent(_askReadyPanel);
        }

        public override void Disable() {
            RemovePanelFromSubContent(_waitingPanel);
        }

        protected override void BindData() {
            _notifyReadyViewModel.OnReady += OnReady;
        }

        protected override void UnbindData() {
            _notifyReadyViewModel.OnReady -= OnReady;
        }

        private void AddPanelToSubContent(UiPanel panel) =>
            _subContentPanel.AddUiPanel(panel);

        private void RemovePanelFromSubContent(UiPanel panel) =>
            _subContentPanel.RemoveUiPanel(panel);

        private void OnReady() =>
            SwitchPanels();

        private void SwitchPanels() {
            RemovePanelFromSubContent(_askReadyPanel);
            AddPanelToSubContent(_waitingPanel);
        }
    }

    public class AskReadyPanel : UiPanel
    {
        private Button _readyButton;

        private readonly IAskReadyViewModel _askReadyViewModel;

        public AskReadyPanel(VisualTreeAsset panelAsset, IAskReadyViewModel askReadyViewModel)
            : base(panelAsset) {

            _askReadyViewModel = askReadyViewModel;
        }

        protected override void OnConstruction() {
            _readyButton =
                _panel.Q<Button>(Constants.VisualElementNames.GameUI.PreparingPanel.AskReadyPanel.ReadyButton);
        }

        public override void Enable() {
            _readyButton.RegisterCallback<ClickEvent>(OnClickReady);
        }

        public override void Disable() {
            _readyButton.UnregisterCallback<ClickEvent>(OnClickReady);
        }

        private void OnClickReady(ClickEvent evt) =>
            _askReadyViewModel.OnClickReady();
    }

    public class WaitingPanel : UiPanel
    {
        public WaitingPanel(VisualTreeAsset panelAsset)
            : base(panelAsset) { }
    }
}
