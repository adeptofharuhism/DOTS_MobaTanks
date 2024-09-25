using Assets.CodeBase.Utility;

namespace Assets.CodeBase.Infrastructure.Services.ConnectionInfo
{
    public class ConnectionInfoService : IConnectionInfoService
    {
        private const string LocalHostIp = "127.0.0.1";
        private const string JoinPortHardcoded = "7979";
        private const string PlayerNameHardcoded = "Bingus";

        public ReactiveProperty<string> ConnectionIp => _connectionIp;
        public ReactiveProperty<ushort> ConnectionPort => _connectionPort;

        public string LocalIp => LocalHostIp;
        public ReactiveProperty<ushort> LocalPort => _localPort;

        public ReactiveProperty<string> PlayerName => _playerName;

        private readonly ReactiveProperty<string> _connectionIp = new();
        private readonly ReactiveProperty<ushort> _connectionPort = new();

        private readonly ReactiveProperty<ushort> _localPort = new();

        private readonly ReactiveProperty<string> _playerName = new();

        public ConnectionInfoService() {
            _connectionIp.Value = LocalHostIp;
            _connectionPort.Value = ushort.Parse(JoinPortHardcoded);

            _localPort.Value = ushort.Parse(JoinPortHardcoded);

            _playerName.Value = PlayerNameHardcoded;
        }

        public void SetConnectionIp(string ip) =>
            _connectionIp.Value = ip;

        public void SetConnectionPort(ushort port) =>
            _connectionPort.Value = port;

        public void SetLocalPort(ushort port) =>
            _localPort.Value = port;

        public void SetPlayerName(string name) =>
            _playerName.Value = name;
    }
}
