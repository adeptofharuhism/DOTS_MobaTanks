using Assets.CodeBase.Inventory;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.CodeBase.UI.MainScene.Panels
{
	public class InventoryButton
	{
		private const string StyleClass = "itemButton";

		public VisualElement VisualElement => _button;
		public Button Button => _button;
		
		private int _itemId;
		
		private readonly Button _button;

		public InventoryButton() {
			_button = new Button();
			_button.AddToClassList(StyleClass);
			
			_itemId = InventorySlot.UndefinedItem;
		}

		public void ChangeItem(int itemId, Texture2D image) {
			_itemId = itemId;

			_button.style.backgroundImage =
				itemId == InventorySlot.UndefinedItem
					? null
					: image;
		}
	}
}