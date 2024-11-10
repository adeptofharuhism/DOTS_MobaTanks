using Assets.CodeBase.Inventory.Items;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.CodeBase.Infrastructure.Services.ItemDescriptionAccess
{
	public class ItemDescriptionAccessService : IItemDescriptionAccess
	{
		public int TotalItemAmount => _itemCollection.ItemDescriptions.Count;
		public List<ItemDescription> Items => _itemCollection.ItemDescriptions;
		public List<GroupedByTypeItemDescriptions> GroupedItemDescriptions =>
			_itemCollection.GroupedByTypeItemDescriptions;
		
		private readonly ItemCollection _itemCollection;

		public ItemDescriptionAccessService(ItemCollection itemCollection) {
			_itemCollection = itemCollection;
		}

		public Texture2D GetImageForItem(int itemId) =>
			_itemCollection.ItemDescriptions[itemId].Image;
	}
}