using Assets.CodeBase.Infrastructure.Services.ItemDescriptionAccess;
using Assets.CodeBase.Inventory;
using Assets.CodeBase.Inventory.Items;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.CodeBase.Infrastructure.Services.UiFactories
{
	public abstract class ItemButton
	{
		private const string StyleClass = "itemButton";

		public VisualElement VisualElement => _button;

		protected IStyle ButtonStyle => _button.style;
		
		private readonly int _buttonParameter;
		private readonly Action<int> _onClick;
		private readonly Button _button;

		protected ItemButton(Action<int> onClick, int buttonParameter) {
			_onClick = onClick;
			_buttonParameter = buttonParameter;

			_button = new Button();
			_button.AddToClassList(StyleClass);
			_button.RegisterCallback<ClickEvent>(OnClickButton);
		}

		private void OnClickButton(ClickEvent evt) {
			_onClick?.Invoke(_buttonParameter);
		}
	}

	public class InventoryButton : ItemButton
	{
		public int ItemId {
			get => _itemId;
			set {
				_itemId = value;

				ButtonStyle.backgroundImage =
					_itemId == InventorySlot.UndefinedItem
						? null
						: _itemDescriptionAccess.GetImageForItem(_itemId);
			}
		}

		private int _itemId;
		
		private readonly IItemDescriptionAccess _itemDescriptionAccess;

		public InventoryButton(Action<int> onClick, int slotId, IItemDescriptionAccess itemDescriptionAccess)
			: base(onClick, slotId) {
			
			_itemDescriptionAccess = itemDescriptionAccess;
			
			ItemId = InventorySlot.UndefinedItem;
		}
	}

	public class ShopButton : ItemButton
	{
		public int ItemCost => _itemCost;
		
		private readonly int _itemCost;
		
		public ShopButton(Action<int> onClick, int itemId, ItemDescription itemDescription)
			: base(onClick, itemId) {

			_itemCost = itemDescription.Cost;
			ButtonStyle.backgroundImage = itemDescription.Image;
		}

		public void Enable() {
			VisualElement.SetEnabled(true);
		}

		public void Disable() {
			VisualElement.SetEnabled(false);
		}
	}
}