using Assets.CodeBase.Infrastructure.Services.MainSceneModeNotifier;
using Assets.CodeBase.Infrastructure.Services.WorldEvents;
using Assets.CodeBase.Utility.StateMachine;

namespace Assets.CodeBase.Infrastructure.GameStateManagement.States
{
    public class PrepareForGameState : IState, IGameState
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IMainSceneModeNotifier _mainSceneModeNotifier;
        private readonly IWorldEventBusService _worldEventBus;

        public PrepareForGameState(
            IGameStateMachine gameStateMachine,
            IMainSceneModeNotifier mainSceneModeNotifier,
            IWorldEventBusService worldEventBusService) {

            _gameStateMachine = gameStateMachine;
            _mainSceneModeNotifier = mainSceneModeNotifier;
            _worldEventBus = worldEventBusService;
        }

        public void Enter() {
            _mainSceneModeNotifier.SetMode(MainSceneMode.Preparing);
            SubscribeToGameStartEvent();
        }

        public void Exit() {
            UnsubscribeFromGameStartEvent();
        }

        private void SubscribeToGameStartEvent() =>
            _worldEventBus.OnStartGame += OnGameStart;

        private void UnsubscribeFromGameStartEvent() =>
            _worldEventBus.OnStartGame -= OnGameStart;

        private void OnGameStart() =>
            _gameStateMachine.EnterGameState<InGameState>();
    }
}
