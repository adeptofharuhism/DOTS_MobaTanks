using Assets.CodeBase.Finances;
using Assets.CodeBase.GameStates.GameStart;
using Assets.CodeBase.Infrastructure.Services.WorldAccess;
using Assets.CodeBase.Shop;
using Assets.CodeBase.Teams;
using Assets.CodeBase.UI;
using Assets.CodeBase.Utility;
using System;

namespace Assets.CodeBase.Infrastructure.Services.WorldEvents
{
    public class WorldEventService : IWorldEventBusService, IWorldEventSubscriptionControlService
    {
        public event Action OnLoadedSubScene;
        public event Action OnStartGame;
        public event Action<TeamType> OnEndGame;

        public IReactiveGetter<bool> ShopAvailability => _shopAvailability;
        public IReactiveGetter<int> MoneyAmount => _moneyAmount;

        private ReactiveProperty<bool> _shopAvailability = new();
        private ReactiveProperty<int> _moneyAmount = new();

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
                .Money.OnChanged += ChangeMoneyAmount;

            _worldAccess.DefaultWorld
                .GetExistingSystemManaged<ShopAvailabilityCheckSystem>()
                .ShopAvailability.OnChanged += ChangeShopAvailability;
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

            _worldAccess.DefaultWorld
                .GetExistingSystemManaged<ClientMoneyUpdateSystem>()
                .Money.OnChanged -= ChangeMoneyAmount;

            _worldAccess.DefaultWorld
                .GetExistingSystemManaged<ShopAvailabilityCheckSystem>()
                .ShopAvailability.OnChanged -= ChangeShopAvailability;
        }

        private void InvokeOnLoadedSubScene() =>
            OnLoadedSubScene?.Invoke();

        private void InvokeOnGameStart() =>
            OnStartGame?.Invoke();

        private void InvokeOnEndGame(TeamType type) =>
            OnEndGame?.Invoke(type);

        private void ChangeMoneyAmount(int money) =>
            _moneyAmount.Value = money;

        private void ChangeShopAvailability(bool availability) =>
            _shopAvailability.Value = availability;
    }
}
