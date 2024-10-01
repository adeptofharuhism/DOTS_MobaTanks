using Assets.CodeBase.Infrastructure.Services.ConnectionInfo;
using Assets.CodeBase.Infrastructure.StateMachine;
using Assets.CodeBase.Infrastructure.StateMachine.States;
using Assets.CodeBase.Utility;
using System;
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

    public interface IStartSceneViewModel
    {
        ReactiveProperty<StartSceneMode> Mode { get; }

        ReactiveProperty<string> JoinPortView { get; }
        ReactiveProperty<string> JoinIpView { get; }
        ReactiveProperty<string> HostPortView { get; }
        ReactiveProperty<string> PlayerNameView { get; }

        void OnClickCancel();
        void OnClickExit();
        void OnClickHostChoice();
        void OnClickJoinChoice();
        void OnClickPlay();
        void OnFocusOutIp(string ip);
        void OnFocusOutPlayerName(string name);
        void OnFocusOutPort(string port);
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

        private readonly IGameStateMachine _gameStateMachine;
        private readonly IConnectionInfoService _connectionInfoService;

        [Inject]
        public StartSceneViewModel(IGameStateMachine gameStateMachine, IConnectionInfoService connectionInfoService) {
            _gameStateMachine = gameStateMachine;
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

        public void OnClickHostChoice() =>
            _mode.Value = StartSceneMode.Host;

        public void OnClickJoinChoice() =>
            _mode.Value = StartSceneMode.Join;

        public void OnClickExit() =>
            Application.Quit();

        public void OnClickPlay() {
            if (_mode.Value == StartSceneMode.ConnectionChoice)
                return;

            bool isHost = false;

            if (_mode.Value == StartSceneMode.Host)
                isHost = true;

            _gameStateMachine.Enter<LoadMainSceneState, bool>(isHost);
        }

        public void OnFocusOutPlayerName(string name) => 
            _connectionInfoService.SetPlayerName(name);

        public void OnFocusOutPort(string port) { 
            ushort parsedPort = ushort.Parse(port);

            switch (_mode.Value) {
                case StartSceneMode.Host:
                    _connectionInfoService.SetLocalPort(parsedPort);
                    break;
                case StartSceneMode.Join:
                    _connectionInfoService.SetConnectionPort(parsedPort);
                    break;
            }
        }

        public void OnFocusOutIp(string ip) {
            if (_mode.Value == StartSceneMode.Join)
                _connectionInfoService.SetConnectionIp(ip);
        }

        public void OnClickCancel() =>
            _mode.Value = StartSceneMode.ConnectionChoice;

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
