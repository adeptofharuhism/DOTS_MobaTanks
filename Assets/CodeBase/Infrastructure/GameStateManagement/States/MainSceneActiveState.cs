using Assets.CodeBase.Infrastructure.Services.WorldControl;
using Assets.CodeBase.Utility.StateMachine;

namespace Assets.CodeBase.Infrastructure.GameStateManagement.States
{
    public class MainSceneActiveState : IState , IGameState
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IWorldControlService _worldControlService;

        public MainSceneActiveState(IGameStateMachine gameStateMachine, IWorldControlService worldControlService) {
            _gameStateMachine = gameStateMachine;
            _worldControlService = worldControlService;
        }

        public void Enter() {
            _worldControlService.StartWorlds();
        }

        public void Exit() {
            _worldControlService.DisposeNetworkWorlds();
        }
    }
}
