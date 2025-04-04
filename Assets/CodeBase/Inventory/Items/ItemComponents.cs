﻿using Assets.CodeBase.Teams;
using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Inventory.Items
{
	[GhostComponent(PrefabType = GhostPrefabType.Server)]
	public struct ItemCommandTag : IComponentData
	{ }

	[GhostComponent(PrefabType = GhostPrefabType.Server)]
	public struct ItemCreationTag : IComponentData
	{ }

	[GhostComponent(PrefabType = GhostPrefabType.Server)]
	public struct ItemRemovalTag : IComponentData
	{ }

	[GhostComponent(PrefabType = GhostPrefabType.Server)]
	public struct ItemCreationPrefabElement : IBufferElementData
	{
		public int BuyCost;
		public Entity Command;
	}

	[GhostComponent(PrefabType = GhostPrefabType.Server)]
	public struct SpawnableItem : IComponentData
	{
		public Entity Value;
	}

	[GhostComponent(PrefabType = GhostPrefabType.Server)]
	public struct VehicleWithItem : IComponentData
	{
		public Entity Value;
	}

	[GhostComponent(PrefabType = GhostPrefabType.Server)]
	public struct InstantiatedItem : IComponentData
	{
		public Entity Value;
	}

	[GhostComponent(PrefabType = GhostPrefabType.Server)]
	public struct SpawnableItemSettings : IComponentData
	{
		public int InventorySlot;
		public TeamType ItemTeam;
		public Entity PlayerEntity;
		public Entity SpawnParent;
	}

	[GhostComponent(PrefabType = GhostPrefabType.Server)]
	public struct ItemRemovalPrefabElement : IBufferElementData
	{
		public int SellCost;
		public Entity Item;
	}

	[GhostComponent(PrefabType = GhostPrefabType.Server)]
	public struct EmptyItemCommand : IComponentData
	{
		public Entity Command;
	}
}
