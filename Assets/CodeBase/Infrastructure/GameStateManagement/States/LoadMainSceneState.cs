using Assets.CodeBase.Infrastructure.Services.SceneLoader;
using Assets.CodeBase.Infrastructure.Services.WorldControl;
using Assets.CodeBase.UI.Curtain;
using Assets.CodeBase.Utility.StateMachine;
using UnityEngine.SceneManagement;

namespace Assets.CodeBase.Infrastructure.GameStateManagement.States
{
    public class LoadMainSceneState : IPayloadedState<bool>, IGameState
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ISceneLoader _sceneLoader;
        private readonly IWorldControlService _worldControlService;
        private readonly ILoadingCurtain _loadingCurtain;

        public LoadMainSceneState(
            IGameStateMachine gameStateMachine,
            ISceneLoader sceneLoader,
            IWorldControlService worldControlService,
            ILoadingCurtain loadingCurtain) {

            _gameStateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
            _worldControlService = worldControlService;
            _loadingCurtain = loadingCurtain;
        }

        public void Enter(bool isHost) {
            _loadingCurtain.Show();

            StartWorlds(isHost);
            DisposeDefaultWorld();

            _sceneLoader.Load(Constants.SceneNames.MainSceneName, LoadSceneMode.Single, OnSceneLoaded);
        }

        public void Exit() { }

        private void StartWorlds(bool isHost) {
            if (isHost)
                _worldControlService.CreateServerWorld();
            _worldControlService.CreateClientWorld();
        }

        private void DisposeDefaultWorld() {
            _worldControlService.DisposeDefaultWorld();
        }

        private void OnSceneLoaded() {
            _loadingCurtain.Hide();
            _gameStateMachine.EnterGameState<MainSceneActiveState>();
        }
    }
}
