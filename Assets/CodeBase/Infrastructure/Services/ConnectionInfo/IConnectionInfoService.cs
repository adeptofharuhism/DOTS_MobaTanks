namespace Assets.CodeBase.Infrastructure.Services.ConnectionInfo
{
    public interface IConnectionInfoService
    {
        string ConnectionIP { get; }
        ushort ConnectionPort { get; }
        string LocalHost { get; }
        string PlayerName { get; }
    }
}