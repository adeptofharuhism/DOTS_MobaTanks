using Assets.CodeBase.Infrastructure.Services.ItemDescriptionAccess;
using Assets.CodeBase.Inventory;
using System;
using UnityEngine.UIElements;

namespace Assets.CodeBase.Infrastructure.Services.UiFactories
{
	public class InventoryButton
	{
		private const string StyleClass = "itemButton";

		public VisualElement VisualElement => _button;

		public int ItemId {
			get => _itemId;
			set {
				_itemId = value;

				_button.style.backgroundImage =
					_itemId == InventorySlot.UndefinedItem
						? null
						: _itemDescriptionAccess.GetImageForItem(_itemId);
			}
		}

		private int _itemId;

		private readonly int _slotId;
		private readonly Action<int> _onClick;
		private readonly Button _button;

		private readonly IItemDescriptionAccess _itemDescriptionAccess;

		public InventoryButton(Action<int> onClick, int slotId, IItemDescriptionAccess itemDescriptionAccess) {
			_onClick = onClick;
			_slotId = slotId;

			_button = new Button();
			_button.AddToClassList(StyleClass);
			_button.RegisterCallback<ClickEvent>(OnClickButton);

			_itemDescriptionAccess = itemDescriptionAccess;

			ItemId = InventorySlot.UndefinedItem;
		}

		private void OnClickButton(ClickEvent evt) {
			_onClick?.Invoke(_slotId);
		}
	}
}