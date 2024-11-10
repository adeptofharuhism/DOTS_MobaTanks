using Assets.CodeBase.Infrastructure.Services.ItemDescriptionAccess;
using Assets.CodeBase.Inventory.Items;
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Assets.CodeBase.Infrastructure.Services.UiFactories
{
	public class ItemButtonFactory : IInventoryButtonFactory, IShopButtonFactory
	{
		private readonly IItemDescriptionAccess _itemDescriptionAccess;
		private readonly VisualTreeAsset _itemGroup;

		public ItemButtonFactory(IItemDescriptionAccess itemDescriptionAccess, VisualTreeAsset itemGroup) {
			_itemDescriptionAccess = itemDescriptionAccess;
			_itemGroup = itemGroup;
		}

		public InventoryButton CreateButton(Action<int> onClick, int slotId) =>
			new(onClick, slotId, _itemDescriptionAccess);

		public VisualElement[] CreateItemGroups(Action<int> onClickShopButton, out ShopButton[] sortedShopButtons) {
			List<GroupedByTypeItemDescriptions> groupedItemDescriptions =
				_itemDescriptionAccess.GroupedItemDescriptions;

			int itemTypeAmount = groupedItemDescriptions.Count;

			VisualElement[] itemGroups = new VisualElement[itemTypeAmount];
			ShopButton[] shopButtons = new ShopButton[_itemDescriptionAccess.TotalItemAmount];
			int nextShopButtonIndex = 0;

			for (int i = 0; i < itemTypeAmount; i++) {
				VisualElement newItemGroup = _itemGroup.Instantiate();

				Label itemGroupNameLabel =
					newItemGroup.Q<Label>(Constants.VisualElementNames.GameUI.InGamePanel.ItemGroup.ItemGroupName);

				itemGroupNameLabel.text = groupedItemDescriptions[i].ItemTypeName;

				VisualElement itemContainer =
					newItemGroup
						.Q<VisualElement>(Constants.VisualElementNames.GameUI.InGamePanel.ItemGroup.ItemContainer);

				foreach (ItemDescription itemDescription in groupedItemDescriptions[i].ItemDescriptions) {
					ShopButton newButton = new(onClickShopButton, itemDescription.Id, itemDescription);
					newButton.Disable();

					itemContainer.Add(newButton.VisualElement);

					shopButtons[nextShopButtonIndex++] = newButton;
				}

				itemGroups[i] = newItemGroup;
			}

			Array.Sort(shopButtons, (x, y) => x.ItemCost > y.ItemCost ? 1 : -1);

			sortedShopButtons = shopButtons;

			return itemGroups;
		}
	}

	public interface IInventoryButtonFactory
	{
		InventoryButton CreateButton(Action<int> onClick, int slotId);
	}

	public interface IShopButtonFactory
	{
		VisualElement[] CreateItemGroups(Action<int> onClickShopButton, out ShopButton[] sortedShopButtons);
	}
}