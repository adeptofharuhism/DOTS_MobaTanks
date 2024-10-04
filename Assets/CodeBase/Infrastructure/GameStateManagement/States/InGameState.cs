using Assets.CodeBase.GameStates.GameStart;
using Assets.CodeBase.Infrastructure.Services.MainSceneModeNotifier;
using Assets.CodeBase.Infrastructure.Services.WinnerNotifier;
using Assets.CodeBase.Teams;
using Assets.CodeBase.Utility.StateMachine;
using Unity.Entities;

namespace Assets.CodeBase.Infrastructure.GameStateManagement.States
{
    public class InGameState : IState, IGameState
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IMainSceneModeNotifier _mainSceneModeNotifier;
        private readonly IWinnerNotifier _winnerNotifier;

        public InGameState(
            IGameStateMachine gameStateMachine,
            IMainSceneModeNotifier mainSceneModeNotifier,
            IWinnerNotifier winnerNotifier) {

            _gameStateMachine = gameStateMachine;
            _mainSceneModeNotifier = mainSceneModeNotifier;
            _winnerNotifier = winnerNotifier;
        }

        public void Enter() {
            _mainSceneModeNotifier.SetMode(MainSceneMode.InGame);
            SubscribeToEndGameEvent();
        }

        public void Exit() {
            UnsubscribeFromEndGameEvent();
        }

        private void SubscribeToEndGameEvent() =>
            World.DefaultGameObjectInjectionWorld
                .GetExistingSystemManaged<ClientEnterEndGameSystem>()
                .OnEndGame += OnEndGame;

        private void UnsubscribeFromEndGameEvent() =>
            World.DefaultGameObjectInjectionWorld
                .GetExistingSystemManaged<ClientEnterEndGameSystem>()
                .OnEndGame -= OnEndGame;

        private void OnEndGame(TeamType winner) {
            _winnerNotifier.NotifyWinnerTeam(winner);
            _gameStateMachine.EnterGameState<GameOverState>();
        }
    }
}
