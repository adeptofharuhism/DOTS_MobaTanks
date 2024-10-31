using Assets.CodeBase.Infrastructure.GameStateManagement;
using Assets.CodeBase.Infrastructure.GameStateManagement.States;
using Assets.CodeBase.Infrastructure.Services.Inventory;
using Assets.CodeBase.Infrastructure.Services.MainSceneModeNotifier;
using Assets.CodeBase.Infrastructure.Services.WorldCommandSender;
using Assets.CodeBase.Infrastructure.Services.WorldEvents;
using Assets.CodeBase.Teams;
using Assets.CodeBase.Utility;
using Assets.CodeBase.Utility.MVVM;
using System;

namespace Assets.CodeBase.UI.MainScene
{
	public interface IEndGameModeViewModel
	{
		event Action<TeamType> OnEndGame;

		void OnClickDisconnect();
	}

	public interface IItemRequestViewModel
	{
		void BuyItem(int itemId);
		void SellItem(int slot);
		void SwapItems(int slotFrom, int slotTo);
		IReactiveGetter<int> MoneyView { get; }
	}

	public interface IShopActivationViewModel
	{
		IReactiveGetter<bool> ShopCanBeShown { get; }
		IReactiveGetter<string> MoneyTextView { get; }
	}

	public interface IInGameModeViewModel : IShopActivationViewModel, IItemRequestViewModel { }

	public interface IAskReadyViewModel
	{
		void OnClickReady();
	}

	public interface INotifyReadyViewModel
	{
		event Action OnReady;
	}

	public interface IPreparingModeViewModel : IAskReadyViewModel, INotifyReadyViewModel { }

	public interface IMainSceneViewModel : IPreparingModeViewModel, IInGameModeViewModel, IEndGameModeViewModel
	{
		IReactiveGetter<MainSceneMode> Mode { get; }
	}

	public class MainSceneViewModel : ViewModel, IMainSceneViewModel
	{
		public event Action OnReady;
		public event Action<TeamType> OnEndGame;

		public IReactiveGetter<MainSceneMode> Mode => _mode;

		public IReactiveGetter<bool> ShopCanBeShown => _shopCanBeShown;

		public IReactiveGetter<int> MoneyView => _moneyView;
		public IReactiveGetter<string> MoneyTextView => _moneyTextView;
		public IReactiveGetter<int> InventorySizeView => _inventorySizeView;

		private readonly ReactiveProperty<MainSceneMode> _mode = new();

		private readonly ReactiveProperty<bool> _shopCanBeShown = new();
		private readonly ReactiveProperty<int> _moneyView = new();
		private readonly ReactiveProperty<string> _moneyTextView = new();
		private readonly ReactiveProperty<int> _inventorySizeView = new();

		private readonly IGameStateMachine _gameStateMachine;
		private readonly IMainSceneModeNotifier _mainSceneModeNotifier;
		private readonly IWorldRpcSenderService _worldRpcSenderService;
		private readonly IWorldEventBusService _worldEventBus;
		private readonly IInventoryService _inventoryService;

		public MainSceneViewModel(
			IGameStateMachine gameStateMachine,
			IMainSceneModeNotifier mainSceneModeNotifier,
			IWorldRpcSenderService worldRpcSenderService,
			IWorldEventBusService worldEventBusService,
			IInventoryService inventoryService) {

			_gameStateMachine = gameStateMachine;
			_mainSceneModeNotifier = mainSceneModeNotifier;
			_worldRpcSenderService = worldRpcSenderService;
			_worldEventBus = worldEventBusService;
			_inventoryService = inventoryService;
		}

		public void OnClickReady() {
			if (_mode.Value != MainSceneMode.Preparing)
				return;


			_worldRpcSenderService.SendReadyRpc();

			OnReady?.Invoke();
		}

		public void BuyItem(int itemId) =>
			_worldRpcSenderService.SendBuyItemRpc(itemId);

		public void SellItem(int slot) =>
			_worldRpcSenderService.SendSellItemRpc(slot);

		public void SwapItems(int slotFrom, int slotTo) {
			_worldRpcSenderService.SendSwapSlotsRpc(slotFrom, slotTo);
		}

		public void OnClickDisconnect() {
			if (_mode.Value != MainSceneMode.EndGame)
				return;


			_gameStateMachine.EnterGameState<LoadStartSceneState>();
		}

		protected override void GetModelValues() {
			_mode.Value = _mainSceneModeNotifier.Mode.Value;
			_moneyTextView.Value = _worldEventBus.MoneyAmount.Value.ToString();
			_shopCanBeShown.Value = _worldEventBus.ShopAvailability.Value;
		}

		protected override void SubscribeToModel() {
			_mainSceneModeNotifier.Mode.OnChanged += ChangeMode;
			_worldEventBus.MoneyAmount.OnChanged += UpdateMoneyView;
			_worldEventBus.ShopAvailability.OnChanged += UpdateShopAvailability;
			_worldEventBus.OnEndGame += InvokeOnEndGame;

			_inventoryService.InventorySize.OnChanged += UpdateInventorySize;
			_inventoryService.OnChangedItem += UpdateItem;
		}

		protected override void UnsubscribeFromModel() {
			_mainSceneModeNotifier.Mode.OnChanged -= ChangeMode;
			_worldEventBus.MoneyAmount.OnChanged -= UpdateMoneyView;
			_worldEventBus.ShopAvailability.OnChanged -= UpdateShopAvailability;
			_worldEventBus.OnEndGame -= InvokeOnEndGame;

			_inventoryService.InventorySize.OnChanged -= UpdateInventorySize;
			_inventoryService.OnChangedItem -= UpdateItem;
		}

		private void ChangeMode(MainSceneMode mode) =>
			_mode.Value = mode;

		private void UpdateMoneyView(int money) {
			_moneyView.Value = money;
			_moneyTextView.Value = money.ToString();
		}

		private void UpdateShopAvailability(bool availability) =>
			_shopCanBeShown.Value = availability;

		private void InvokeOnEndGame(TeamType type) =>
			OnEndGame?.Invoke(type);

		private void UpdateInventorySize(int size) {
			UnityEngine.Debug.Log($"Set inventory size as {size}");
		}

		private void UpdateItem(int slotId, int itemId) {
			UnityEngine.Debug.Log($"Update of slot {slotId}: value is {itemId}");
		}
	}
}