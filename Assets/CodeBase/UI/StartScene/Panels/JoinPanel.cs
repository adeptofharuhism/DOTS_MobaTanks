using UnityEngine.UIElements;

namespace Assets.CodeBase.UI.StartScene.Panels
{
    public sealed class JoinPanel : ConnectionPanel
    {
        private readonly IJoinVariantViewModel _joinVariantViewModel;

        private Button _joinButton;
        private TextField _joinPort;
        private TextField _ip;

        public JoinPanel(VisualTreeAsset panelAsset, IJoinVariantViewModel joinVariantViewModel)
            : base(panelAsset, joinVariantViewModel) {

            _joinVariantViewModel = joinVariantViewModel;
        }

        public override void Enable() {
            base.Enable();

            _joinButton.RegisterCallback<ClickEvent>(OnClickJoinButton);
            _joinPort.RegisterCallback<FocusOutEvent>(OnFocusOutJoinPort);
            _ip.RegisterCallback<FocusOutEvent>(OnFocusOutIp);
        }

        public override void Disable() {
            base.Disable();

            _joinButton.UnregisterCallback<ClickEvent>(OnClickJoinButton);
            _joinPort.UnregisterCallback<FocusOutEvent>(OnFocusOutJoinPort);
            _ip.UnregisterCallback<FocusOutEvent>(OnFocusOutIp);
        }

        protected override void CacheVisualElements() {
            _joinButton = _panel.Q<Button>(Constants.VisualElementNames.ConnectionMenu.JoinGamePanel.JoinButton);
            _cancelButton = _panel.Q<Button>(Constants.VisualElementNames.ConnectionMenu.JoinGamePanel.CancelButton);
            _playerName = _panel.Q<TextField>(Constants.VisualElementNames.ConnectionMenu.JoinGamePanel.PlayerName);
            _joinPort = _panel.Q<TextField>(Constants.VisualElementNames.ConnectionMenu.JoinGamePanel.JoinPort);
            _ip = _panel.Q<TextField>(Constants.VisualElementNames.ConnectionMenu.JoinGamePanel.JoinIP);
        }

        protected override void ReadInitialViewModelData() {
            base.ReadInitialViewModelData();

            _joinPort.value = _joinVariantViewModel.JoinPortView.Value;
            _ip.value = _joinVariantViewModel.JoinIpView.Value;
        }

        protected override void BindData() {
            base.BindData();

            _joinVariantViewModel.JoinPortView.OnChanged += OnChangedJoinPort;
            _joinVariantViewModel.JoinIpView.OnChanged += OnChangedIp;
        }

        protected override void UnbindData() {
            base.UnbindData();

            _joinVariantViewModel.JoinPortView.OnChanged -= OnChangedJoinPort;
            _joinVariantViewModel.JoinIpView.OnChanged -= OnChangedIp;
        }

        private void OnClickJoinButton(ClickEvent evt) => 
            _joinVariantViewModel.OnClickJoinGame();

        private void OnFocusOutJoinPort(FocusOutEvent evt) => 
            _joinVariantViewModel.OnFocusOutJoinPort(_joinPort.value);

        private void OnFocusOutIp(FocusOutEvent evt) =>
            _joinVariantViewModel.OnFocusOutIp(_ip.value);

        private void OnChangedJoinPort(string port) => 
            _joinPort.value = port;

        private void OnChangedIp(string ip) =>
            _ip.value = ip;
    }
}
