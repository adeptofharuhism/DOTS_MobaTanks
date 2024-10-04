using Assets.CodeBase.Infrastructure.Services.MainSceneModeNotifier;
using Assets.CodeBase.Utility.StateMachine;

namespace Assets.CodeBase.Infrastructure.GameStateManagement.States
{
    public class GameOverState : IState, IGameState
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IMainSceneModeNotifier _mainSceneModeNotifier;

        public GameOverState(IGameStateMachine gameStateMachine, IMainSceneModeNotifier mainSceneModeNotifier) {
            _gameStateMachine = gameStateMachine;
            _mainSceneModeNotifier = mainSceneModeNotifier;
        }

        public void Enter() {
            _mainSceneModeNotifier.SetMode(MainSceneMode.GameOver);
        }

        public void Exit() { }
    }
}
