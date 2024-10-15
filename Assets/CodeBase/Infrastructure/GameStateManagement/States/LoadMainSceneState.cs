using Assets.CodeBase.Infrastructure.Services.MainSceneModeNotifier;
using Assets.CodeBase.Infrastructure.Services.SceneLoader;
using Assets.CodeBase.Infrastructure.Services.WorldControl;
using Assets.CodeBase.Infrastructure.Services.WorldEvents;
using Assets.CodeBase.UI.Curtain;
using Assets.CodeBase.Utility.StateMachine;
using UnityEngine.SceneManagement;

namespace Assets.CodeBase.Infrastructure.GameStateManagement.States
{
    public class LoadMainSceneState : IPayloadedState<bool>, IGameState
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ISceneLoader _sceneLoader;
        private readonly IWorldControlService _worldControl;
        private readonly IWorldEventBusService _worldEventBus;
        private readonly ILoadingCurtain _loadingCurtain;
        private readonly IMainSceneModeNotifier _mainSceneModeNotifier;

        public LoadMainSceneState(
            IGameStateMachine gameStateMachine,
            ISceneLoader sceneLoader,
            IWorldControlService worldControlService,
            IWorldEventBusService worldEventBus,
            ILoadingCurtain loadingCurtain,
            IMainSceneModeNotifier mainSceneModeNotifier) {

            _gameStateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
            _worldControl = worldControlService;
            _worldEventBus = worldEventBus;
            _loadingCurtain = loadingCurtain;
            _mainSceneModeNotifier = mainSceneModeNotifier;
        }

        public void Enter(bool isHost) {
            _loadingCurtain.Show();

            _mainSceneModeNotifier.SetMode(MainSceneMode.Loading);

            CreateWorlds(isHost);
            DisposeDefaultWorld();

            _sceneLoader.Load(Constants.SceneNames.MainSceneName, LoadSceneMode.Single, OnSceneLoaded);
        }

        public void Exit() { }

        private void CreateWorlds(bool isHost) {
            if (isHost)
                _worldControl.CreateServerWorld();
            _worldControl.CreateClientWorld();
        }

        private void DisposeDefaultWorld() {
            _worldControl.DisposeDefaultWorld();
        }

        private void OnSceneLoaded() {
            SubscribeToWorldLoadingEvent();
            _worldControl.StartWorlds();
        }

        private void SubscribeToWorldLoadingEvent() =>
            _worldEventBus.OnLoadedSubScene += OnLoadedSubScene;

        private void OnLoadedSubScene() {
            UnsubscribeFromWorldLoadingEvent();

            _loadingCurtain.Hide();

            _gameStateMachine.EnterGameState<PrepareForGameState>();
        }

        private void UnsubscribeFromWorldLoadingEvent() =>
            _worldEventBus.OnLoadedSubScene -= OnLoadedSubScene;
    }
}
