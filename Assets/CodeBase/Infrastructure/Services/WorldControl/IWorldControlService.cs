namespace Assets.CodeBase.Infrastructure.Services.WorldControl
{
    public interface IWorldControlService
    {
        void CreateWorlds();
        void DisposeWorlds();
        void SetHost(bool isHost);
    }
}