using UnityEngine.UIElements;

namespace Assets.CodeBase.UI.StartScene.Panels
{
    public abstract class ConnectionPanel : StartSceneUiPanel
    {
        protected Button _playButton;
        protected Button _cancelButton;
        protected TextField _playerName;
        protected TextField _port;

        protected ConnectionPanel(VisualTreeAsset panelAsset, IStartSceneViewModel startSceneViewModel)
            : base(panelAsset, startSceneViewModel) { }

        public override void Enable() {
            _playButton.RegisterCallback<ClickEvent>(OnClickPlayButton);
            _cancelButton.RegisterCallback<ClickEvent>(OnClickCancelButton);
            _playerName.RegisterCallback<FocusOutEvent>(OnFocusOutPlayerName);
            _port.RegisterCallback<FocusOutEvent>(OnFocusOutPort);
        }

        public override void Disable() {
            _playButton.UnregisterCallback<ClickEvent>(OnClickPlayButton);
            _cancelButton.UnregisterCallback<ClickEvent>(OnClickCancelButton);
            _playerName.UnregisterCallback<FocusOutEvent>(OnFocusOutPlayerName);
            _port.UnregisterCallback<FocusOutEvent>(OnFocusOutPort);
        }

        private void OnClickPlayButton(ClickEvent evt) =>
            _startSceneViewModel.OnClickPlay();

        private void OnClickCancelButton(ClickEvent evt) =>
            _startSceneViewModel.OnClickCancel();

        private void OnFocusOutPlayerName(FocusOutEvent evt) =>
            _startSceneViewModel.OnFocusOutPlayerName(_playerName.value);

        private void OnFocusOutPort(FocusOutEvent evt) =>
            _startSceneViewModel.OnFocusOutPort(_port.value);

        protected void OnChangedPlayerName(string name) =>
            _playerName.value = name;

        protected void OnChangedPort(string port) =>
            _port.value = port;
    }
}
