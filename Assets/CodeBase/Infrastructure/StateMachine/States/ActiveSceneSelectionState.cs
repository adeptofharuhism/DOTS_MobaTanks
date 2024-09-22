using UnityEngine.SceneManagement;

namespace Assets.CodeBase.Infrastructure.StateMachine.States
{
    public class ActiveSceneSelectionState : IState
    {
        private readonly IGameStateMachine _gameStateMachine;

        public ActiveSceneSelectionState(IGameStateMachine gameStateMachine) {
            _gameStateMachine = gameStateMachine;
        }

        public void Enter() {
            EnterNextState();
        }

        public void Exit() { }

        private void EnterNextState() {
            switch (SceneManager.GetActiveScene().name) {
                case Constants.SceneNames.MainSceneName:
                    _gameStateMachine.Enter<MainSceneActiveState>();
                    break;
                case Constants.SceneNames.StartSceneName:
                default:
                    _gameStateMachine.Enter<StartSceneActiveState>();
                    break;
            }
        }
    }
}
