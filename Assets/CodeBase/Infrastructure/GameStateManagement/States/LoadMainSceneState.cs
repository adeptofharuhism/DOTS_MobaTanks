using Assets.CodeBase.Infrastructure.Services.MainSceneModeNotifier;
using Assets.CodeBase.Infrastructure.Services.SceneLoader;
using Assets.CodeBase.Infrastructure.Services.WorldControl;
using Assets.CodeBase.UI;
using Assets.CodeBase.UI.Curtain;
using Assets.CodeBase.Utility.StateMachine;
using Unity.Entities;
using UnityEngine.SceneManagement;

namespace Assets.CodeBase.Infrastructure.GameStateManagement.States
{
    public class LoadMainSceneState : IPayloadedState<bool>, IGameState
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ISceneLoader _sceneLoader;
        private readonly IWorldControlService _worldControlService;
        private readonly ILoadingCurtain _loadingCurtain;
        private readonly IMainSceneModeNotifier _mainSceneModeNotifier;

        public LoadMainSceneState(
            IGameStateMachine gameStateMachine,
            ISceneLoader sceneLoader,
            IWorldControlService worldControlService,
            ILoadingCurtain loadingCurtain,
            IMainSceneModeNotifier mainSceneModeNotifier) {

            _gameStateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
            _worldControlService = worldControlService;
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
                _worldControlService.CreateServerWorld();
            _worldControlService.CreateClientWorld();
        }

        private void DisposeDefaultWorld() {
            _worldControlService.DisposeDefaultWorld();
        }

        private void OnSceneLoaded() {
            SubscribeToWorldLoadingEvent();
            _worldControlService.StartWorlds();
        }

        private void SubscribeToWorldLoadingEvent() =>
            World.DefaultGameObjectInjectionWorld
                .GetExistingSystemManaged<DeployUiOnClientSystem>()
                .OnReadyForUiDeploy += OnSubSceneLoaded;

        private void OnSubSceneLoaded() {
            UnsubscribeFromWorldLoadingEvent();

            _loadingCurtain.Hide();

            _gameStateMachine.EnterGameState<PrepareForGameState>();
        }

        private void UnsubscribeFromWorldLoadingEvent() =>
            World.DefaultGameObjectInjectionWorld
                .GetExistingSystemManaged<DeployUiOnClientSystem>()
                .OnReadyForUiDeploy -= OnSubSceneLoaded;
    }
}
