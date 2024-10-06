using UnityEngine.UIElements;

namespace Assets.CodeBase.UI.StartScene.Panels
{
    public sealed class ConnectionChoicePanel : UiPanel
    {
        private readonly IConnectionChoiceViewModel _connectionChoiceViewModel;

        private Button _hostButton;
        private Button _joinButton;
        private Button _exitButton;

        public ConnectionChoicePanel(VisualTreeAsset panelAsset, IConnectionChoiceViewModel connectionChoiceViewModel)
            : base(panelAsset) {

            _connectionChoiceViewModel = connectionChoiceViewModel;
        }

        public override void Enable() {
            _hostButton.RegisterCallback<ClickEvent>(OnClickHostButton);
            _joinButton.RegisterCallback<ClickEvent>(OnClickJoinButton);
            _exitButton.RegisterCallback<ClickEvent>(OnClickExitButton);
        }

        public override void Disable() {
            _hostButton.UnregisterCallback<ClickEvent>(OnClickHostButton);
            _joinButton.UnregisterCallback<ClickEvent>(OnClickJoinButton);
            _exitButton.UnregisterCallback<ClickEvent>(OnClickExitButton);
        }

        protected override void CacheVisualElements() {
            _hostButton = _panel.Q<Button>(Constants.VisualElementNames.ConnectionMenu.ConnectionChoicePanel.HostButton);
            _joinButton = _panel.Q<Button>(Constants.VisualElementNames.ConnectionMenu.ConnectionChoicePanel.JoinButton);
            _exitButton = _panel.Q<Button>(Constants.VisualElementNames.ConnectionMenu.ConnectionChoicePanel.ExitButton);
        }

        private void OnClickHostButton(ClickEvent evt) =>
            _connectionChoiceViewModel.OnClickHostConnectionVariant();

        private void OnClickJoinButton(ClickEvent evt) =>
            _connectionChoiceViewModel.OnClickJoinConnectionVariant();

        private void OnClickExitButton(ClickEvent evt) =>
            _connectionChoiceViewModel.OnClickExit();
    }
}
