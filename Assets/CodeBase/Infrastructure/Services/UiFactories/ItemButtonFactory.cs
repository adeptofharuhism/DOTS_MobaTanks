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
			new InventoryButton(onClick, slotId, _itemDescriptionAccess);


		public (VisualElement[], ShopButton[]) CreateItemGroups() {
			ItemType[] itemTypes = _itemDescriptionAccess.GetItemTypes();
//я писал создание групп предметов для магазина
			List<ItemDescription> itemDescriptions = _itemDescriptionAccess.GetItems();
		}
	}

	public interface IInventoryButtonFactory
	{
		InventoryButton CreateButton(Action<int> onClick, int slotId);
	}

	public interface IShopButtonFactory
	{
		(VisualElement[], ShopButton[]) CreateItemGroups();
	}
}