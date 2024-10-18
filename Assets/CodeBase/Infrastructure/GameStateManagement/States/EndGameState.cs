using Assets.CodeBase.Infrastructure.Services.MainSceneModeNotifier;
using Assets.CodeBase.Utility.StateMachine;

namespace Assets.CodeBase.Infrastructure.GameStateManagement.States
{
    public class EndGameState : IState, IGameState
    {
        private readonly IMainSceneModeNotifier _mainSceneModeNotifier;

        public EndGameState(IMainSceneModeNotifier mainSceneModeNotifier) {
            _mainSceneModeNotifier = mainSceneModeNotifier;
        }

        public void Enter() {
            _mainSceneModeNotifier.SetMode(MainSceneMode.EndGame);
        }

        public void Exit() { }
    }
}
