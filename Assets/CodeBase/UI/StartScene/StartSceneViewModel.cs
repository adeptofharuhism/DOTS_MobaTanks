using Assets.CodeBase.Infrastructure.Services.ConnectionInfo;
using Assets.CodeBase.Utility;
using System;
using Zenject;

namespace Assets.CodeBase.UI.StartScene
{
    public enum StartSceneMode
    {
        ConnectionChoice,
        Join,
        Host
    }

    public interface IStartSceneViewModel
    {
        ReactiveProperty<string> JoinPortView { get; }
        ReactiveProperty<string> JoinIpView { get; }
        ReactiveProperty<string> HostPortView { get; }
        ReactiveProperty<string> PlayerNameView { get; }

        void OnClickExit();
        void OnClickHostChoice();
        void OnClickJoinChoice();
    }

    public class StartSceneViewModel : IStartSceneViewModel, IInitializable, IDisposable
    {
        public ReactiveProperty<StartSceneMode> Mode => _mode;

        public ReactiveProperty<string> JoinPortView => _joinPortView;
        public ReactiveProperty<string> JoinIpView => _joinIpView;
        public ReactiveProperty<string> HostPortView => _hostPortView;
        public ReactiveProperty<string> PlayerNameView => _playerNameView;

        private readonly ReactiveProperty<StartSceneMode> _mode = new();

        private readonly ReactiveProperty<string> _joinPortView = new();
        private readonly ReactiveProperty<string> _joinIpView = new();
        private readonly ReactiveProperty<string> _hostPortView = new();
        private readonly ReactiveProperty<string> _playerNameView = new();

        private readonly IConnectionInfoService _connectionInfoService;

        public StartSceneViewModel(IConnectionInfoService connectionInfoService) {
            _connectionInfoService = connectionInfoService;
        }

        public void Initialize() {
            SetInitialMode();
            GetModelValues();
            SubscribeOnModelChanges();
        }

        public void Dispose() {
            UnsubscribeFromModelChanges();
        }

        public void OnClickHostChoice() { }

        public void OnClickJoinChoice() { }

        public void OnClickExit() { }

        private void SetInitialMode() =>
            _mode.Value = StartSceneMode.ConnectionChoice;

        private void GetModelValues() {
            _joinPortView.Value = _connectionInfoService.ConnectionPort.Value.ToString();
            _joinIpView.Value = _connectionInfoService.ConnectionIp.Value;
            _hostPortView.Value = _connectionInfoService.LocalPort.Value.ToString();
            _playerNameView.Value = _connectionInfoService.PlayerName.Value;
        }

        private void SubscribeOnModelChanges() {
            _connectionInfoService.ConnectionPort.OnChanged += OnChangedConnectionPort;
            _connectionInfoService.ConnectionIp.OnChanged += OnChangedConnectionIp;
            _connectionInfoService.LocalPort.OnChanged += OnChangedLocalPort;
            _connectionInfoService.PlayerName.OnChanged += OnChangedPlayerName;
        }

        private void UnsubscribeFromModelChanges() {
            _connectionInfoService.ConnectionPort.OnChanged -= OnChangedConnectionPort;
            _connectionInfoService.ConnectionIp.OnChanged -= OnChangedConnectionIp;
            _connectionInfoService.LocalPort.OnChanged -= OnChangedLocalPort;
            _connectionInfoService.PlayerName.OnChanged -= OnChangedPlayerName;
        }

        private void OnChangedConnectionPort(ushort newPort) =>
            _joinPortView.Value = newPort.ToString();

        private void OnChangedConnectionIp(string newIp) =>
            _joinIpView.Value = newIp;

        private void OnChangedLocalPort(ushort newPort) =>
            _hostPortView.Value = newPort.ToString();

        private void OnChangedPlayerName(string newPlayerName) =>
            _playerNameView.Value = newPlayerName;
    }
}
