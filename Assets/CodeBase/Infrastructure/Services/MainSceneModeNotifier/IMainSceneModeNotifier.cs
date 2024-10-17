using Assets.CodeBase.Utility;

namespace Assets.CodeBase.Infrastructure.Services.MainSceneModeNotifier
{
    public enum MainSceneMode
    {
        Loading,
        Preparing,
        InGame,
        GameOver
    }

    public interface IMainSceneModeNotifier
    {
        IReactiveGetter<MainSceneMode> Mode { get; }

        void SetMode(MainSceneMode mode);
    }
}
