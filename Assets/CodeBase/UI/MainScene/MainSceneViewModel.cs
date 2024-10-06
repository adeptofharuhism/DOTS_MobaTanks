using Assets.CodeBase.Infrastructure.Services.MainSceneModeNotifier;
using Assets.CodeBase.Utility;
using Assets.CodeBase.Utility.MVVM;

namespace Assets.CodeBase.UI.MainScene
{
    public interface IMainSceneViewModel
    {
        ReactiveProperty<MainSceneMode> Mode { get; }
    }

    public class MainSceneViewModel : ViewModel, IMainSceneViewModel
    {
        public ReactiveProperty<MainSceneMode> Mode => _mode;

        private readonly ReactiveProperty<MainSceneMode> _mode = new();

        private readonly IMainSceneModeNotifier _mainSceneModeNotifier;

        public MainSceneViewModel(IMainSceneModeNotifier mainSceneModeNotifier) {
            _mainSceneModeNotifier = mainSceneModeNotifier;
        }

        protected override void GetModelValues() {
            _mode.Value = _mainSceneModeNotifier.Mode.Value;
        }

        protected override void SubscribeToModel() {
            _mainSceneModeNotifier.Mode.OnChanged += OnChangedMode;
        }

        protected override void UnsubscribeFromModel() {
            _mainSceneModeNotifier.Mode.OnChanged -= OnChangedMode;
        }

        private void OnChangedMode(MainSceneMode mode) =>
            _mode.Value = mode;
    }
}
