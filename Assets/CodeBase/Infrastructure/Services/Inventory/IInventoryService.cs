using Assets.CodeBase.Utility;
using System;
using UnityEngine;

namespace Assets.CodeBase.Infrastructure.Services.Inventory
{
	public interface IInventoryService
	{
		event Action<int, int> OnChangedItem;
		IReactiveGetter<int> InventorySize { get; }
	}
}