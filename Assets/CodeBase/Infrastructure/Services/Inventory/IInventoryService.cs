using Assets.CodeBase.Utility;
using System;

namespace Assets.CodeBase.Infrastructure.Services.Inventory
{
	public interface IInventoryService
	{
		event Action<int, int> OnChangedItem;
		IReactiveGetter<int> InventorySize { get; }
	}
}