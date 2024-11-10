using Assets.CodeBase.Inventory.Items;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.CodeBase.Infrastructure.Services.ItemDescriptionAccess
{
	public interface IItemDescriptionAccess
	{
		List<ItemDescription> Items { get; }
		List<GroupedByTypeItemDescriptions> GroupedItemDescriptions { get; }
		int TotalItemAmount { get; }
		Texture2D GetImageForItem(int itemId);
	}
}