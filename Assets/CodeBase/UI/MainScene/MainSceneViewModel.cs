using Assets.CodeBase.Infrastructure.Services.MainSceneModeNotifier;
using Assets.CodeBase.Infrastructure.Services.WorldCommandSender;
using Assets.CodeBase.Infrastructure.Services.WorldEvents;
using Assets.CodeBase.Utility;
using Assets.CodeBase.Utility.MVVM;
using System;

namespace Assets.CodeBase.UI.MainScene
{
    public interface IShopViewModel
    {
        IReactiveGetter<bool> ShopCanBeShown { get; }
        IReactiveGetter<string> MoneyView { get; }
    }

    public interface IInGameModeViewModel : IShopViewModel { }

    public interface IAskReadyViewModel
    {
        void OnClickReady();
    }

    public interface INotifyReadyViewModel
    {
        event Action OnReady;
    }

    public interface IPreparingModeViewModel : IAskReadyViewModel, INotifyReadyViewModel { }

    public interface IMainSceneViewModel : IPreparingModeViewModel, IInGameModeViewModel
    {
        IReactiveGetter<MainSceneMode> Mode { get; }
    }

    public class MainSceneViewModel : ViewModel, IMainSceneViewModel
    {
        public event Action OnReady;

        public IReactiveGetter<MainSceneMode> Mode => _mode;

        public IReactiveGetter<bool> ShopCanBeShown => _shopCanBeShown;
        public IReactiveGetter<string> MoneyView => _moneyView;

        private readonly ReactiveProperty<MainSceneMode> _mode = new();

        private readonly ReactiveProperty<bool> _shopCanBeShown = new();
        private readonly ReactiveProperty<string> _moneyView = new();

        private readonly IMainSceneModeNotifier _mainSceneModeNotifier;
        private readonly IWorldRpcSenderService _worldRpcSenderService;
        private readonly IWorldEventBusService _worldEventBus;

        public MainSceneViewModel(
            IMainSceneModeNotifier mainSceneModeNotifier,
            IWorldRpcSenderService worldRpcSenderService,
            IWorldEventBusService worldEventBusService) {

            _mainSceneModeNotifier = mainSceneModeNotifier;
            _worldRpcSenderService = worldRpcSenderService;
            _worldEventBus = worldEventBusService;
        }

        public void OnClickReady() {
            _worldRpcSenderService.SendReadyRpc();

            OnReady?.Invoke();
        }

        protected override void GetModelValues() {
            _mode.Value = _mainSceneModeNotifier.Mode.Value;
            _moneyView.Value = _worldEventBus.MoneyAmount.Value.ToString();
            _shopCanBeShown.Value = _worldEventBus.ShopAvailability.Value;
        }

        protected override void SubscribeToModel() {
            _mainSceneModeNotifier.Mode.OnChanged += ChangeMode;
            _worldEventBus.MoneyAmount.OnChanged += UpdateMoneyView;
            _worldEventBus.ShopAvailability.OnChanged += UpdateShopAvailability;
        }

        protected override void UnsubscribeFromModel() {
            _mainSceneModeNotifier.Mode.OnChanged -= ChangeMode;
            _worldEventBus.MoneyAmount.OnChanged -= UpdateMoneyView;
            _worldEventBus.ShopAvailability.OnChanged -= UpdateShopAvailability;
        }

        private void ChangeMode(MainSceneMode mode) =>
            _mode.Value = mode;

        private void UpdateMoneyView(int money) =>
            _moneyView.Value = money.ToString();

        private void UpdateShopAvailability(bool availability) =>
            _shopCanBeShown.Value = availability;
    }
}
