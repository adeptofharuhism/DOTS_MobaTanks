using Assets.CodeBase.Infrastructure.GameStateManagement;
using Assets.CodeBase.Infrastructure.GameStateManagement.States;
using Assets.CodeBase.Infrastructure.Services.ConnectionInfo;
using Assets.CodeBase.Utility;
using Assets.CodeBase.Utility.MVVM;
using UnityEngine;
using Zenject;

namespace Assets.CodeBase.UI.StartScene
{
    public enum StartSceneMode
    {
        ConnectionChoice,
        Join,
        Host
    }

    public class StartSceneViewModel : ViewModel, IStartSceneViewModel
    {
        public IReactiveGetter<StartSceneMode> Mode => _mode;

        public IReactiveGetter<string> JoinPortView => _joinPortView;
        public IReactiveGetter<string> JoinIpView => _joinIpView;
        public IReactiveGetter<string> HostPortView => _hostPortView;
        public IReactiveGetter<string> PlayerNameView => _playerNameView;

        private readonly ReactiveProperty<StartSceneMode> _mode = new();

        private readonly ReactiveProperty<string> _joinPortView = new();
        private readonly ReactiveProperty<string> _joinIpView = new();
        private readonly ReactiveProperty<string> _hostPortView = new();
        private readonly ReactiveProperty<string> _playerNameView = new();

        private readonly IGameStateMachine _gameStateMachine;
        private readonly IConnectionInfoService _connectionInfoService;

        [Inject]
        public StartSceneViewModel(IGameStateMachine gameStateMachine, IConnectionInfoService connectionInfoService) {
            _gameStateMachine = gameStateMachine;
            _connectionInfoService = connectionInfoService;
        }

        public override void Initialize() {
            SetInitialMode();

            base.Initialize();
        }

        public void OnClickHostConnectionVariant() =>
            _mode.Value = StartSceneMode.Host;

        public void OnClickJoinConnectionVariant() =>
            _mode.Value = StartSceneMode.Join;

        public void OnClickExit() =>
            Application.Quit();

        public void OnFocusOutPlayerName(string name) =>
            _connectionInfoService.SetPlayerName(name);

        public void OnClickCancel() =>
            _mode.Value = StartSceneMode.ConnectionChoice;

        public void OnClickHostGame() {
            if (_mode.Value != StartSceneMode.Host)
                return;

            StartGameConnection(isHost: true);
        }

        public void OnFocusOutHostPort(string port) {
            if (_mode.Value != StartSceneMode.Host)
                return;

            _connectionInfoService.SetLocalPort(ParsePort(port));
        }

        public void OnClickJoinGame() {
            if (_mode.Value != StartSceneMode.Join)
                return;

            StartGameConnection(isHost: false);
        }

        public void OnFocusOutJoinPort(string port) {
            if (_mode.Value != StartSceneMode.Join)
                return;

            _connectionInfoService.SetConnectionPort(ParsePort(port));
        }

        public void OnFocusOutIp(string ip) {
            if (_mode.Value != StartSceneMode.Join)
                return;

            _connectionInfoService.SetConnectionIp(ip);
        }

        protected override void GetModelValues() {
            _joinPortView.Value = _connectionInfoService.ConnectionPort.Value.ToString();
            _joinIpView.Value = _connectionInfoService.ConnectionIp.Value;
            _hostPortView.Value = _connectionInfoService.LocalPort.Value.ToString();
            _playerNameView.Value = _connectionInfoService.PlayerName.Value;
        }

        protected override void SubscribeToModel() {
            _connectionInfoService.ConnectionPort.OnChanged += OnChangedConnectionPort;
            _connectionInfoService.ConnectionIp.OnChanged += OnChangedConnectionIp;
            _connectionInfoService.LocalPort.OnChanged += OnChangedLocalPort;
            _connectionInfoService.PlayerName.OnChanged += OnChangedPlayerName;
        }

        protected override void UnsubscribeFromModel() {
            _connectionInfoService.ConnectionPort.OnChanged -= OnChangedConnectionPort;
            _connectionInfoService.ConnectionIp.OnChanged -= OnChangedConnectionIp;
            _connectionInfoService.LocalPort.OnChanged -= OnChangedLocalPort;
            _connectionInfoService.PlayerName.OnChanged -= OnChangedPlayerName;
        }

        private void SetInitialMode() =>
            _mode.Value = StartSceneMode.ConnectionChoice;

        private void StartGameConnection(bool isHost) =>
            _gameStateMachine.EnterGameState<LoadMainSceneState, bool>(isHost);

        private ushort ParsePort(string port) =>
            ushort.Parse(port);

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
