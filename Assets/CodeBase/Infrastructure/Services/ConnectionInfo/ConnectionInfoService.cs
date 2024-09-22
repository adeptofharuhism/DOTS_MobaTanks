namespace Assets.CodeBase.Infrastructure.Services.ConnectionInfo
{
    public class ConnectionInfoService : IConnectionInfoService
    {
        private const string LocalHostIP = "127.0.0.1";
        private const string JoinPortHardcoded = "7979";
        private const string PlayerNameHardcoded = "Bingus";

        public string LocalHost => LocalHostIP;

        public string ConnectionIP => LocalHost;
        public ushort ConnectionPort => ushort.Parse(JoinPortHardcoded);

        public string PlayerName => PlayerNameHardcoded;
    }
}
