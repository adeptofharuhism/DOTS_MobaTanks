using Assets.CodeBase.Infrastructure.Services.SceneLoader;
using Assets.CodeBase.Infrastructure.Services.WorldControl;
using UnityEngine.SceneManagement;

namespace Assets.CodeBase.Infrastructure.StateMachine.States
{
    public class LoadMainSceneState : IPayloadedState<bool>
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ISceneLoader _sceneLoader;
        private readonly IWorldControlService _worldControlService;

        public LoadMainSceneState(
            IGameStateMachine gameStateMachine,
            ISceneLoader sceneLoader,
            IWorldControlService worldControlService) {

            _gameStateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
            _worldControlService = worldControlService;
        }

        public void Enter(bool isHost) {
            StartWorlds(isHost);
            DisposeDefaultWorld();

            _sceneLoader.Load(Constants.SceneNames.MainSceneName, LoadSceneMode.Additive, OnSceneLoaded);
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
            _gameStateMachine.Enter<MainSceneActiveState>();
        }
    }
}
