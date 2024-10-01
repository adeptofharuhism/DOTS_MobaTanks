using UnityEngine.UIElements;

namespace Assets.CodeBase.UI.StartScene.Panels
{
    public sealed class ConnectionChoicePanel : StartSceneUiPanel
    {
        private Button _hostButton;
        private Button _joinButton;
        private Button _exitButton;

        public ConnectionChoicePanel(VisualTreeAsset panelAsset, IStartSceneViewModel startSceneViewModel)
            : base(panelAsset, startSceneViewModel) { }

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
            _startSceneViewModel.OnClickHostChoice();

        private void OnClickJoinButton(ClickEvent evt) =>
            _startSceneViewModel.OnClickJoinChoice();

        private void OnClickExitButton(ClickEvent evt) =>
            _startSceneViewModel.OnClickExit();
    }
}
