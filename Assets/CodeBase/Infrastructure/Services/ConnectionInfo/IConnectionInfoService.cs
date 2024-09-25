using Assets.CodeBase.Utility;

namespace Assets.CodeBase.Infrastructure.Services.ConnectionInfo
{
    public interface IConnectionInfoService
    {
        ReactiveProperty<string> ConnectionIp { get; }
        ReactiveProperty<ushort> ConnectionPort { get; }

        string LocalIp { get; }
        ReactiveProperty<ushort> LocalPort { get; }

        ReactiveProperty<string> PlayerName { get; }

        void SetConnectionIp(string ip);
        void SetConnectionPort(ushort port);
        void SetLocalPort(ushort port);
        void SetPlayerName(string name);
    }
}