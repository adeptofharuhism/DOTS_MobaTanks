using Assets.CodeBase.Infrastructure.Services.SceneLoader;
using Assets.CodeBase.UI.Curtain;
using UnityEngine.SceneManagement;

namespace Assets.CodeBase.Infrastructure.StateMachine.States
{
    public class LoadStartSceneState : IState
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ISceneLoader _sceneLoader;
        private readonly ILoadingCurtain _loadingCurtain;

        public LoadStartSceneState(IGameStateMachine gameStateMachine, ISceneLoader sceneLoader, ILoadingCurtain loadingCurtain) {
            _gameStateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
            _loadingCurtain = loadingCurtain;
        }

        public void Enter() {
            _loadingCurtain.Show();
            _sceneLoader.Load(Constants.SceneNames.StartSceneName, LoadSceneMode.Single, OnSceneLoaded);
        }

        public void Exit() { }

        private void OnSceneLoaded() {
            _loadingCurtain.Hide();
            _gameStateMachine.Enter<StartSceneActiveState>();
        }
    }
}
