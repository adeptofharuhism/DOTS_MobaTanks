using Assets.CodeBase.Infrastructure.Services.MainSceneModeNotifier;
using Assets.CodeBase.Utility.MVVM;
using Zenject;

namespace Assets.CodeBase.UI.MainScene
{
    public class MainSceneView : View
    {
        private IMainSceneViewModel _mainSceneViewModel;

        [Inject]
        public void Construct(IMainSceneViewModel mainSceneViewModel) {
            _mainSceneViewModel = mainSceneViewModel;
        }

        public override void Initialize() {
            base.Initialize();
        }

        protected override void SubscribeToViewModel() {
            _mainSceneViewModel.Mode.OnChanged += OnChangeMode;
        }

        protected override void UnsubscribeFromViewModel() {
            _mainSceneViewModel.Mode.OnChanged -= OnChangeMode;
        }

        private void OnChangeMode(MainSceneMode mode) {
            switch (mode) {
                default:
                case MainSceneMode.Loading:
                    break;
                case MainSceneMode.Preparing:
                    break;
                case MainSceneMode.InGame:
                    break;
                case MainSceneMode.GameOver:
                    break;
            }
        }
    }
}
