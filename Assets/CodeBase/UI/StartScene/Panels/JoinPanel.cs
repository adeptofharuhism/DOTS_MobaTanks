using UnityEngine.UIElements;

namespace Assets.CodeBase.UI.StartScene.Panels
{
    public sealed class JoinPanel : ConnectionPanel
    {
        private TextField _ip;

        public JoinPanel(VisualTreeAsset panelAsset, IStartSceneViewModel startSceneViewModel)
            : base(panelAsset, startSceneViewModel) { }

        public override void Enable() {
            base.Enable();

            _ip.RegisterCallback<FocusOutEvent>(OnFocusOutIp);
        }

        public override void Disable() {
            base.Disable();

            _ip.UnregisterCallback<FocusOutEvent>(OnFocusOutIp);
        }

        protected override void CacheVisualElements() {
            _playButton = _panel.Q<Button>(Constants.VisualElementNames.ConnectionMenu.JoinGamePanel.JoinButton);
            _cancelButton = _panel.Q<Button>(Constants.VisualElementNames.ConnectionMenu.JoinGamePanel.CancelButton);
            _playerName = _panel.Q<TextField>(Constants.VisualElementNames.ConnectionMenu.JoinGamePanel.PlayerName);
            _port = _panel.Q<TextField>(Constants.VisualElementNames.ConnectionMenu.JoinGamePanel.JoinPort);
            _ip = _panel.Q<TextField>(Constants.VisualElementNames.ConnectionMenu.JoinGamePanel.JoinIP);
        }

        protected override void BindData() {
            _startSceneViewModel.PlayerNameView.OnChanged += OnChangedPlayerName;
            _startSceneViewModel.JoinPortView.OnChanged += OnChangedPort;
            _startSceneViewModel.JoinIpView.OnChanged += OnChangedIp;
        }

        protected override void UnbindData() {
            _startSceneViewModel.PlayerNameView.OnChanged -= OnChangedPlayerName;
            _startSceneViewModel.JoinPortView.OnChanged -= OnChangedPort;
            _startSceneViewModel.JoinIpView.OnChanged -= OnChangedIp;
        }

        private void OnFocusOutIp(FocusOutEvent evt) =>
            _startSceneViewModel.OnFocusOutIp(_ip.value);

        private void OnChangedIp(string ip) =>
            _ip.value = ip;
    }
}
