using System;
using UnityEngine.UIElements;

namespace Assets.CodeBase.UI.StartScene.Panels
{
    public sealed class HostPanel : ConnectionPanel
    {
        public HostPanel(VisualTreeAsset panelAsset, IStartSceneViewModel startSceneViewModel)
            : base(panelAsset, startSceneViewModel) { }

        protected override void CacheVisualElements() {
            _playButton = _panel.Q<Button>(Constants.VisualElementNames.ConnectionMenu.HostGamePanel.HostButton);
            _cancelButton = _panel.Q<Button>(Constants.VisualElementNames.ConnectionMenu.HostGamePanel.CancelButton);
            _playerName = _panel.Q<TextField>(Constants.VisualElementNames.ConnectionMenu.HostGamePanel.PlayerName);
            _port = _panel.Q<TextField>(Constants.VisualElementNames.ConnectionMenu.HostGamePanel.JoinPort);
        }

        protected override void BindData() {
            _startSceneViewModel.PlayerNameView.OnChanged += OnChangedPlayerName;
            _startSceneViewModel.HostPortView.OnChanged += OnChangedPort;
        }

        protected override void UnbindData() {
            _startSceneViewModel.PlayerNameView.OnChanged -= OnChangedPlayerName;
            _startSceneViewModel.HostPortView.OnChanged -= OnChangedPort;
        }
    }
}
