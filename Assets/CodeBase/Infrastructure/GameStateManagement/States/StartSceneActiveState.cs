using Assets.CodeBase.Utility.StateMachine;

namespace Assets.CodeBase.Infrastructure.GameStateManagement.States
{
    public class StartSceneActiveState : IState, IGameState
    {
        private readonly IGameStateMachine _gameStateMachine;

        public StartSceneActiveState(IGameStateMachine gameStateMachine) {
            _gameStateMachine = gameStateMachine;
        }

        public void Enter() { }

        public void Exit() { }
    }
}
