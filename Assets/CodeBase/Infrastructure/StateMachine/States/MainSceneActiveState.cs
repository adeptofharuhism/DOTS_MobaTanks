using Assets.CodeBase.Infrastructure.Services.WorldControl;

namespace Assets.CodeBase.Infrastructure.StateMachine.States
{
    public class MainSceneActiveState : IState
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IWorldControlService _worldControlService;

        public MainSceneActiveState(IGameStateMachine gameStateMachine, IWorldControlService worldControlService) {
            _gameStateMachine = gameStateMachine;
            _worldControlService = worldControlService;
        }

        public void Enter() {
            _worldControlService.CreateWorlds();
        }

        public void Exit() {
            _worldControlService.SetHost(false);
            _worldControlService.DisposeWorlds();
        }
    }
}
