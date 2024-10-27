using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Inventory
{
	//Settings
	public struct BasicInventoryCapacity : IComponentData
	{
		public int Value;
	}

	//Inventory components
	[GhostComponent(PrefabType = GhostPrefabType.Server)]
	public struct InventoryInitializationTag : IComponentData { }

	[GhostComponent(PrefabType = GhostPrefabType.Server)]
	public struct InventoryTag : IComponentData { }

	[GhostComponent(PrefabType = GhostPrefabType.Server)]
	public struct ItemSlotCollection : ICleanupComponentData
	{
		public NativeArray<InventorySlot> Slots;
	}

	[GhostComponent(PrefabType = GhostPrefabType.Server)]
	public struct InventorySlotEntity : IComponentData
	{
		public Entity Value;
	}

	public struct InventorySlot
	{
		public const int UndefinedItem = -1;

		public int ItemId;
		public bool IsSpawnable;
		public Entity SpawnedItem;
	}

	public struct GhostInventorySlot : IBufferElementData
	{
		[GhostField] public int ItemId;
	}

	//Rpcs
	public struct BuyItemRpc : IRpcCommand
	{
		public int ItemId;
	}

	public struct SellItemRpc : IRpcCommand
	{
		public int Slot;
	}

	public struct SwapSlotsRpc : IRpcCommand
	{
		public int FromSlot;
		public int ToSlot;
	}
}