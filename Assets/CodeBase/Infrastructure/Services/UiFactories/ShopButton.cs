using Assets.CodeBase.Inventory.Items;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.CodeBase.Infrastructure.Services.UiFactories
{
	public class ShopButton
	{
		public VisualElement VisualElement => _visualElement;

		public int ItemCost => _itemCost;

		private readonly int _itemId;
		private readonly Action<int> _onClick;
		private readonly int _itemCost;
		private readonly VisualElement _visualElement;

		public ShopButton(Action<int> onClick, ItemDescription itemDescription, VisualTreeAsset shopButtonAsset) {
			_onClick = onClick;
			_itemId = itemDescription.Id;
			_itemCost = itemDescription.Cost;

			_visualElement = shopButtonAsset.Instantiate();
				
			Button button =
				_visualElement.Q<Button>(Constants.VisualElementNames.GameUI.InGamePanel.ShopButtonTemplate.Button);
			
			button.RegisterCallback<ClickEvent>(OnClickButton);
			button.style.backgroundImage = itemDescription.Image;
			
			Label label =
				_visualElement.Q<Label>(Constants.VisualElementNames.GameUI.InGamePanel.ShopButtonTemplate.Label);
			
			label.text = _itemCost.ToString();
		}

		public void Enable() {
			_visualElement.SetEnabled(true);
		}

		public void Disable() {
			_visualElement.SetEnabled(false);
		}

		private void OnClickButton(ClickEvent evt) {
			_onClick?.Invoke(_itemId);
		}
	}
}