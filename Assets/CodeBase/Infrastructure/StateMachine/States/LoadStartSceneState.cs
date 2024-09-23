using Assets.CodeBase.Infrastructure.Services.SceneLoader;
using UnityEngine.SceneManagement;

namespace Assets.CodeBase.Infrastructure.StateMachine.States
{
    public class LoadStartSceneState : IState
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ISceneLoader _sceneLoader;

        public LoadStartSceneState(IGameStateMachine gameStateMachine, ISceneLoader sceneLoader) {
            _gameStateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
        }

        public void Enter() =>
            _sceneLoader.Load(Constants.SceneNames.StartSceneName, LoadSceneMode.Single, OnSceneLoaded);

        public void Exit() { }

        private void OnSceneLoaded() {
            _gameStateMachine.Enter<StartSceneActiveState>();
        }
    }
}
