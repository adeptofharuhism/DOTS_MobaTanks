using Assets.CodeBase.Infrastructure.Services.MainSceneModeNotifier;
using Assets.CodeBase.Infrastructure.Services.WorldEvents;
using Assets.CodeBase.Teams;
using Assets.CodeBase.Utility.StateMachine;

namespace Assets.CodeBase.Infrastructure.GameStateManagement.States
{
    public class InGameState : IState, IGameState
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IMainSceneModeNotifier _mainSceneModeNotifier;
        private readonly IWorldEventBusService _worldEventBus;

        public InGameState(
            IGameStateMachine gameStateMachine,
            IMainSceneModeNotifier mainSceneModeNotifier,
            IWorldEventBusService worldEventBusService) {

            _gameStateMachine = gameStateMachine;
            _mainSceneModeNotifier = mainSceneModeNotifier;
            _worldEventBus = worldEventBusService;
        }

        public void Enter() {
            _mainSceneModeNotifier.SetMode(MainSceneMode.InGame);
            SubscribeToEndGameEvent();
        }

        public void Exit() {
            UnsubscribeFromEndGameEvent();
        }

        private void SubscribeToEndGameEvent() =>
            _worldEventBus.OnEndGame += OnEndGame;

        private void UnsubscribeFromEndGameEvent() =>
            _worldEventBus.OnEndGame -= OnEndGame;

        private void OnEndGame(TeamType winner) => 
            _gameStateMachine.EnterGameState<EndGameState>();
    }
}
