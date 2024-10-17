using Assets.CodeBase.Utility;

namespace Assets.CodeBase.Infrastructure.Services.MainSceneModeNotifier
{
    public class MainSceneModeNotifier : IMainSceneModeNotifier
    {
        public IReactiveGetter<MainSceneMode> Mode => _mode;

        private readonly ReactiveProperty<MainSceneMode> _mode = new();

        public MainSceneModeNotifier() { }

        public void SetMode(MainSceneMode mode) =>
            _mode.Value = mode;
    }
}
