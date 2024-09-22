namespace Assets.CodeBase.Infrastructure.StateMachine.States
{
    public class StartSceneActiveState : IState
    {
        private readonly IGameStateMachine _gameStateMachine;

        public StartSceneActiveState(IGameStateMachine gameStateMachine) {
            _gameStateMachine = gameStateMachine;
        }

        public void Enter() { }

        public void Exit() { }
    }
}
