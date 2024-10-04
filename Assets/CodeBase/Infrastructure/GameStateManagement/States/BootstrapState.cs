using Assets.CodeBase.Utility.StateMachine;

namespace Assets.CodeBase.Infrastructure.GameStateManagement.States
{
    public class BootstrapState : IState, IGameState
    {
        private readonly IGameStateMachine _gameStateMachine;

        public BootstrapState(IGameStateMachine gameStateMachine) {
            _gameStateMachine = gameStateMachine;
        }

        public void Enter() => 
            _gameStateMachine.EnterGameState<LoadStartSceneState>();

        public void Exit() { }
    }
}
