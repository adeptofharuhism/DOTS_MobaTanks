using Assets.CodeBase.Infrastructure.Services.UiFactories;
using Assets.CodeBase.Utility.MVVM;
using UnityEngine.UIElements;

namespace Assets.CodeBase.UI.MainScene.Panels
{
	public class ShopPart : UiPart
	{
		private VisualElement _itemGroupContainer;
		private ShopButton[] _sortedShopButtons;
		private int _leastExpensiveDisabledButtonIndex;
		private int _lastMoneyUpdate;

		private readonly IShopViewModel _shopViewModel;
		private readonly IShopButtonFactory _shopButtonFactory;

		public ShopPart(VisualElement parent, IShopViewModel shopViewModel, IShopButtonFactory shopButtonFactory)
			: base(parent) {

			_shopViewModel = shopViewModel;
			_shopButtonFactory = shopButtonFactory;
		}

		protected override void CacheVisualElements() {
			_itemGroupContainer =
				_parent.Q<VisualElement>(Constants.VisualElementNames.GameUI.InGamePanel.ItemGroupContainer);

			VisualElement[] itemGroups = _shopButtonFactory.CreateItemGroups(OnClickShopButton, out _sortedShopButtons);

			foreach (VisualElement itemGroup in itemGroups)
				_itemGroupContainer.Add(itemGroup);
		}

		protected override void ReadInitialViewModelData() {
			UpdateItemGroupContainerVisibility(_shopViewModel.ShopIsVisible.Value);
			UpdateActiveButtons(_shopViewModel.MoneyView.Value);
		}

		protected override void BindData() {
			_shopViewModel.ShopIsVisible.OnChanged += UpdateItemGroupContainerVisibility;
			_shopViewModel.MoneyView.OnChanged += UpdateActiveButtons;
		}

		protected override void RegisterCallbacks() { }

		protected override void UnregisterCallbacks() { }

		protected override void UnbindData() {
			_shopViewModel.ShopIsVisible.OnChanged -= UpdateItemGroupContainerVisibility;
			_shopViewModel.MoneyView.OnChanged -= UpdateActiveButtons;
		}

		private void UpdateItemGroupContainerVisibility(bool isVisible) {
			_itemGroupContainer.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
			_itemGroupContainer.SetEnabled(isVisible);
		}

		private void OnClickShopButton(int itemId) =>
			_shopViewModel.BuyItem(itemId);

		private void UpdateActiveButtons(int money) {
			if (money > _lastMoneyUpdate)
				ActivateAffordableButtons(money);
			else
				DeactivateExpensiveButtons(money);

			_lastMoneyUpdate = money;
		}

		private void ActivateAffordableButtons(int money) {
			while (_leastExpensiveDisabledButtonIndex<_sortedShopButtons.Length) {
				if (money < _sortedShopButtons[_leastExpensiveDisabledButtonIndex].ItemCost)
					return;
				
				_sortedShopButtons[_leastExpensiveDisabledButtonIndex++].Enable();
			}
		}

		private void DeactivateExpensiveButtons(int money) {
			while (_leastExpensiveDisabledButtonIndex>0) {
				if (money >= _sortedShopButtons[_leastExpensiveDisabledButtonIndex-1].ItemCost)
					return;
				
				_sortedShopButtons[_leastExpensiveDisabledButtonIndex-1].Disable();
				_leastExpensiveDisabledButtonIndex--;
			}
		}
	}
}