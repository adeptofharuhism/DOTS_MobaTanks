using Assets.CodeBase.Constants;
using Zenject;

namespace Assets.CodeBase.Infrastructure.StateMachine.States
{
    public class BootstrapState : IState
    {
        private readonly IGameStateMachine _gameStateMachine;

        public BootstrapState(IGameStateMachine gameStateMachine) {
            _gameStateMachine = gameStateMachine;
        }

        public void Enter() => 
            _gameStateMachine.Enter<LoadLevelState, string>(SceneNames.StartSceneName);

        public void Exit() { }
    }
}
