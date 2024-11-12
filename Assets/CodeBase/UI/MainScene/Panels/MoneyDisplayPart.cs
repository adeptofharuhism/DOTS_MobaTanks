using Assets.CodeBase.Constants;
using Assets.CodeBase.Utility.MVVM;
using UnityEngine.UIElements;

namespace Assets.CodeBase.UI.MainScene.Panels
{
	public class MoneyDisplayPart : UiPart
	{
		private Label _moneyLabel;
		private Button _shopButton;

		private readonly IMoneyDisplayViewModel _moneyDisplayViewModel;

		public MoneyDisplayPart(VisualElement parent, IMoneyDisplayViewModel moneyDisplayViewModel)
			: base(parent) {

			_moneyDisplayViewModel = moneyDisplayViewModel;
		}

		protected override void CacheVisualElements() {
			_moneyLabel = _parent.Q<Label>(VisualElementNames.GameUI.InGamePanel.MoneyLabel);
			_shopButton = _parent.Q<Button>(VisualElementNames.GameUI.InGamePanel.ShopButton);
		}

		protected override void ReadInitialViewModelData() {
			UpdateMoneyValue(_moneyDisplayViewModel.MoneyTextView.Value);
		}

		protected override void BindData() {
			_moneyDisplayViewModel.MoneyTextView.OnChanged += UpdateMoneyValue;

		}

		protected override void RegisterCallbacks() {
			_shopButton.RegisterCallback<ClickEvent>(OnClickShop);
		}

		protected override void UnregisterCallbacks() {
			_shopButton.UnregisterCallback<ClickEvent>(OnClickShop);
		}

		protected override void UnbindData() {
			_moneyDisplayViewModel.MoneyTextView.OnChanged -= UpdateMoneyValue;
		}

		private void UpdateMoneyValue(string moneyText) =>
			_moneyLabel.text = moneyText;

		private void OnClickShop(ClickEvent evt) {
			_moneyDisplayViewModel.ClickShop();
		}
	}
}