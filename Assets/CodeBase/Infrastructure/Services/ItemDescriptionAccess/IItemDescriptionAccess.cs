using Assets.CodeBase.Inventory.Items;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.CodeBase.Infrastructure.Services.ItemDescriptionAccess
{
	public interface IItemDescriptionAccess
	{
		Texture2D GetImageForItem(int itemId);
		ItemDescription GetItem(int itemId);
		ItemType[] GetItemTypes();
		List<ItemDescription> GetItems();
	}
}