using Assets.CodeBase.Infrastructure.Services.MainSceneModeNotifier;
using Assets.CodeBase.UI;
using Assets.CodeBase.Utility.StateMachine;
using Unity.Entities;

namespace Assets.CodeBase.Infrastructure.GameStateManagement.States
{
    public class PrepareForGameState : IState, IGameState
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IMainSceneModeNotifier _mainSceneModeNotifier;

        public PrepareForGameState(IGameStateMachine gameStateMachine, IMainSceneModeNotifier mainSceneModeNotifier) {
            _gameStateMachine = gameStateMachine;
            _mainSceneModeNotifier = mainSceneModeNotifier;
        }

        public void Enter() {
            _mainSceneModeNotifier.SetMode(MainSceneMode.Preparing);
            SubscribeToGameStartEvent();
        }

        public void Exit() {
            UnsubscribeFromGameStartEvent();
        }

        private void SubscribeToGameStartEvent() =>
            World.DefaultGameObjectInjectionWorld
                .GetExistingSystemManaged<GameStartNotificationSystem>()
                .OnGameStart += OnGameStart;

        private void UnsubscribeFromGameStartEvent() =>
            World.DefaultGameObjectInjectionWorld
                .GetExistingSystemManaged<GameStartNotificationSystem>()
                .OnGameStart -= OnGameStart;

        private void OnGameStart() =>
            _gameStateMachine.EnterGameState<InGameState>();
    }
}
