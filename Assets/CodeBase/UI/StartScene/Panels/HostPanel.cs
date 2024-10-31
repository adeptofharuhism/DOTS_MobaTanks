using UnityEngine.UIElements;

namespace Assets.CodeBase.UI.StartScene.Panels
{
    public sealed class HostPanel : ConnectionPanel
    {
        private readonly IHostVariantViewModel _hostVariantViewModel;

        private Button _hostButton;
        private TextField _hostPort;

        public HostPanel(VisualTreeAsset panelAsset, IHostVariantViewModel hostVariantViewModel)
            : base(panelAsset, hostVariantViewModel) {

            _hostVariantViewModel = hostVariantViewModel;
        }

        protected override void CacheVisualElements() {
            _hostButton = _panel.Q<Button>(Constants.VisualElementNames.ConnectionMenu.HostGamePanel.HostButton);
            _cancelButton = _panel.Q<Button>(Constants.VisualElementNames.ConnectionMenu.HostGamePanel.CancelButton);
            _playerName = _panel.Q<TextField>(Constants.VisualElementNames.ConnectionMenu.HostGamePanel.PlayerName);
            _hostPort = _panel.Q<TextField>(Constants.VisualElementNames.ConnectionMenu.HostGamePanel.JoinPort);
        }
        
        protected override void RegisterCallbacks() {
            base.RegisterCallbacks();

            _hostButton.RegisterCallback<ClickEvent>(OnClickHostButton);
            _hostPort.RegisterCallback<FocusOutEvent>(OnFocusOutHostPort);
        }

        protected override void UnregisterCallbacks() {
            base.UnregisterCallbacks();

            _hostButton.UnregisterCallback<ClickEvent>(OnClickHostButton);
            _hostPort.UnregisterCallback<FocusOutEvent>(OnFocusOutHostPort);
        }

        protected override void ReadInitialViewModelData() {
            base.ReadInitialViewModelData();

            _hostPort.value = _hostVariantViewModel.HostPortView.Value;
        }

        protected override void BindData() {
            base.BindData();

            _hostVariantViewModel.HostPortView.OnChanged += OnChangedHostPort;
        }

        protected override void UnbindData() {
            base.UnbindData();

            _hostVariantViewModel.HostPortView.OnChanged -= OnChangedHostPort;
        }

        private void OnClickHostButton(ClickEvent evt) =>
            _hostVariantViewModel.OnClickHostGame();

        private void OnFocusOutHostPort(FocusOutEvent evt) =>
            _hostVariantViewModel.OnFocusOutHostPort(_hostPort.value);

        private void OnChangedHostPort(string port) =>
            _hostPort.value = port;
    }
}
