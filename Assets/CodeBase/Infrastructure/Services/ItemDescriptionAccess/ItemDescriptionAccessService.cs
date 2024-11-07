using Assets.CodeBase.Inventory.Items;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.CodeBase.Infrastructure.Services.ItemDescriptionAccess
{
	public class ItemDescriptionAccessService : IItemDescriptionAccess
	{
		private readonly ItemCollection _itemCollection;

		public ItemDescriptionAccessService(ItemCollection itemCollection) {
			_itemCollection = itemCollection;
		}

		public Texture2D GetImageForItem(int itemId) =>
			_itemCollection.ItemDescriptions[itemId].Image;

		public ItemDescription GetItem(int itemId) =>
			_itemCollection.ItemDescriptions[itemId];

		public ItemType[] GetItemTypes() =>
			_itemCollection.ItemTypes;

		public List<ItemDescription> GetItems() =>
			_itemCollection.ItemDescriptions;
	}
}