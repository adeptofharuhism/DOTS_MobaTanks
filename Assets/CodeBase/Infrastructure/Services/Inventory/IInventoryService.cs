using Assets.CodeBase.Utility;
using System;
using UnityEngine;

namespace Assets.CodeBase.Infrastructure.Services.Inventory
{
	public interface IInventoryService
	{
		event Action<int, int, Texture2D> OnChangedItem;
		IReactiveGetter<int> InventorySize { get; }
	}
}