using Assets.CodeBase.Finances;
using Assets.CodeBase.GameStates.GameStart;
using Assets.CodeBase.Infrastructure.Services.WorldAccess;
using Assets.CodeBase.Teams;
using Assets.CodeBase.UI;
using System;

namespace Assets.CodeBase.Infrastructure.Services.WorldEvents
{
    public class WorldEventService : IWorldEventBusService, IWorldEventSubscriptionControlService
    {
        public event Action OnLoadedSubScene;
        public event Action OnStartGame;
        public event Action<TeamType> OnEndGame;
        public event Action<int> OnUpdateMoneyAmount;

        private readonly IWorldAccessService _worldAccess;

        public WorldEventService(IWorldAccessService worldAccessService) {
            _worldAccess = worldAccessService;
        }

        public void SubscribeToWorldEvents() {
            _worldAccess.DefaultWorld
                .GetExistingSystemManaged<DeployUiOnClientSystem>()
                .OnReadyForUiDeploy += InvokeOnLoadedSubScene;

            _worldAccess.DefaultWorld
                .GetExistingSystemManaged<GameStartNotificationSystem>()
                .OnGameStart += InvokeOnGameStart;

            _worldAccess.DefaultWorld
                .GetExistingSystemManaged<ClientEnterEndGameSystem>()
                .OnEndGame += InvokeOnEndGame;


            _worldAccess.DefaultWorld
                .GetExistingSystemManaged<ClientMoneyUpdateSystem>()
                .OnMoneyValueChanged += InvokeUpdateMoneyAmount;
        }

        public void UnsubscribeFromWorldEvents() {
            _worldAccess.DefaultWorld
                .GetExistingSystemManaged<DeployUiOnClientSystem>()
                .OnReadyForUiDeploy -= InvokeOnLoadedSubScene;

            _worldAccess.DefaultWorld
                .GetExistingSystemManaged<GameStartNotificationSystem>()
                .OnGameStart -= InvokeOnGameStart;

            _worldAccess.DefaultWorld
                .GetExistingSystemManaged<ClientEnterEndGameSystem>()
                .OnEndGame -= InvokeOnEndGame;
        }

        private void InvokeOnLoadedSubScene() =>
            OnLoadedSubScene?.Invoke();

        private void InvokeOnGameStart() =>
            OnStartGame?.Invoke();

        private void InvokeOnEndGame(TeamType type) =>
            OnEndGame?.Invoke(type);

        private void InvokeUpdateMoneyAmount(int money) =>
            OnUpdateMoneyAmount?.Invoke(money);
    }
}
