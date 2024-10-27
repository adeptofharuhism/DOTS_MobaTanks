using Assets.CodeBase.Utility;
using ModestTree.Util;
using System;
using Unity.Entities;

namespace Assets.CodeBase.Inventory
{
	[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
	[UpdateInGroup(typeof(InventorySystemGroup))]
	public partial class UpdateClientInventorySystem : SystemBase
	{
		public event Action<int, int> OnChangedItem;

		public ReactiveProperty<int> InventorySize = new();

		private int[] _clientInventory;

		protected override void OnCreate() {
			RequireForUpdate<BasicInventoryCapacity>();
		}

		protected override void OnUpdate() {
			if (_clientInventory == null) {
				int inventoryCapacity = SystemAPI.GetSingleton<BasicInventoryCapacity>().Value;

				_clientInventory = new int[inventoryCapacity];

				for (int i = 0; i < inventoryCapacity; i++)
					_clientInventory[i] = InventorySlot.UndefinedItem;

				InventorySize.Value = inventoryCapacity;
			}

			foreach (DynamicBuffer<GhostInventorySlot> ghostInventory
				in SystemAPI.Query<DynamicBuffer<GhostInventorySlot>>()) {

				for (int i = 0; i < ghostInventory.Length; i++) {
					if (_clientInventory[i] == ghostInventory[i].ItemId)
						continue;


					_clientInventory[i] = ghostInventory[i].ItemId;
					OnChangedItem?.Invoke(i, _clientInventory[i]);
				}
			}
		}
	}
}