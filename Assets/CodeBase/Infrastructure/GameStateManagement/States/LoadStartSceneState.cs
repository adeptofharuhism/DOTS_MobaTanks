using Assets.CodeBase.Infrastructure.Services.SceneLoader;
using Assets.CodeBase.Infrastructure.Services.WorldControl;
using Assets.CodeBase.UI.Curtain;
using Assets.CodeBase.Utility.StateMachine;
using UnityEngine.SceneManagement;

namespace Assets.CodeBase.Infrastructure.GameStateManagement.States
{
    public class LoadStartSceneState : IState, IGameState
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ISceneLoader _sceneLoader;
        private readonly IWorldControlService _worldControlService;
        private readonly ILoadingCurtain _loadingCurtain;

        public LoadStartSceneState(
            IGameStateMachine gameStateMachine,
            ISceneLoader sceneLoader,
            IWorldControlService worldControlService,
            ILoadingCurtain loadingCurtain) {

            _gameStateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
            _worldControlService = worldControlService;
            _loadingCurtain = loadingCurtain;
        }

        public void Enter() {
            _loadingCurtain.Show();

            DisconnectFromServer();

            _sceneLoader.Load(Constants.SceneNames.StartSceneName, LoadSceneMode.Single, OnSceneLoaded);
        }

        public void Exit() { }

        private void DisconnectFromServer() {
            _worldControlService.DisconnectFromServerWorld();
            _worldControlService.DisposeNetworkWorlds();
        }

        private void OnSceneLoaded() {
            _loadingCurtain.Hide();

            _gameStateMachine.EnterGameState<StartSceneActiveState>();
        }
    }
}
