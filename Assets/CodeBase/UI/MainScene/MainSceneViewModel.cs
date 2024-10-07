using Assets.CodeBase.Infrastructure.Services.MainSceneModeNotifier;
using Assets.CodeBase.Infrastructure.Services.WorldCommandSender;
using Assets.CodeBase.Utility;
using Assets.CodeBase.Utility.MVVM;
using System;

namespace Assets.CodeBase.UI.MainScene
{
    public interface IAskReadyViewModel
    {
        void OnClickReady();
    }

    public interface IPreparingModeViewModel : IAskReadyViewModel
    {
        event Action OnReady;
    }

    public interface IMainSceneViewModel : IPreparingModeViewModel
    {
        ReactiveProperty<MainSceneMode> Mode { get; }
    }

    public class MainSceneViewModel : ViewModel, IMainSceneViewModel
    {
        public event Action OnReady;

        public ReactiveProperty<MainSceneMode> Mode => _mode;

        private readonly ReactiveProperty<MainSceneMode> _mode = new();

        private readonly IMainSceneModeNotifier _mainSceneModeNotifier;
        private readonly IWorldRpcSenderService _worldRpcSenderService;

        public MainSceneViewModel(IMainSceneModeNotifier mainSceneModeNotifier, IWorldRpcSenderService worldRpcSenderService) {
            _mainSceneModeNotifier = mainSceneModeNotifier;
            _worldRpcSenderService = worldRpcSenderService;
        }

        public void OnClickReady() {
            _worldRpcSenderService.SendReadyRpc();

            OnReady?.Invoke();
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
