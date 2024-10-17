using Assets.CodeBase.Utility;

namespace Assets.CodeBase.Infrastructure.Services.ConnectionInfo
{
    public interface IConnectionInfoService
    {
        IReactiveGetter<string> ConnectionIp { get; }
        IReactiveGetter<ushort> ConnectionPort { get; }

        string LocalIp { get; }
        IReactiveGetter<ushort> LocalPort { get; }

        IReactiveGetter<string> PlayerName { get; }

        void SetConnectionIp(string ip);
        void SetConnectionPort(ushort port);
        void SetLocalPort(ushort port);
        void SetPlayerName(string name);
    }
}