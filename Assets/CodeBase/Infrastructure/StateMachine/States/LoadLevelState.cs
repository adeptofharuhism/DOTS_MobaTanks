using Assets.CodeBase.Infrastructure.Services.SceneLoader;

namespace Assets.CodeBase.Infrastructure.StateMachine.States
{
    public class LoadLevelState : IPayloadedState<string>
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ISceneLoader _sceneLoader;

        public LoadLevelState(IGameStateMachine gameStateMachine, ISceneLoader sceneLoader) {
            _gameStateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
        }

        public void Enter(string sceneName) =>
            _sceneLoader.Load(sceneName, OnSceneLoaded);

        public void Exit() { }

        private void OnSceneLoaded() {
            _gameStateMachine.Enter<ActiveSceneSelectionState>();
        }
    }
}
