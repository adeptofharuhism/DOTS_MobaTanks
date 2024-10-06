using Assets.CodeBase.Utility.MVVM;
using UnityEngine.UIElements;

namespace Assets.CodeBase.UI.StartScene.Panels
{
    public abstract class ConnectionPanel : UiPanel
    {
        private readonly IConnectionVariantViewModel _connectionVariantViewModel;

        protected Button _cancelButton;
        protected TextField _playerName;

        protected ConnectionPanel(VisualTreeAsset panelAsset, IConnectionVariantViewModel connectionVariantViewModel)
            : base(panelAsset) {

            _connectionVariantViewModel = connectionVariantViewModel;
        }

        public override void Enable() {
            _cancelButton.RegisterCallback<ClickEvent>(OnClickCancelButton);
            _playerName.RegisterCallback<FocusOutEvent>(OnFocusOutPlayerName);
        }

        public override void Disable() {
            _cancelButton.UnregisterCallback<ClickEvent>(OnClickCancelButton);
            _playerName.UnregisterCallback<FocusOutEvent>(OnFocusOutPlayerName);
        }

        protected override void BindData() {
            _connectionVariantViewModel.PlayerNameView.OnChanged += OnChangedPlayerName;
        }

        protected override void UnbindData() {
            _connectionVariantViewModel.PlayerNameView.OnChanged -= OnChangedPlayerName;
        }

        private void OnClickCancelButton(ClickEvent evt) =>
            _connectionVariantViewModel.OnClickCancel();

        private void OnFocusOutPlayerName(FocusOutEvent evt) =>
            _connectionVariantViewModel.OnFocusOutPlayerName(_playerName.value);

        private void OnChangedPlayerName(string name) =>
            _playerName.value = name;
    }
}
