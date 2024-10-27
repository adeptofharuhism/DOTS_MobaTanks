using Assets.CodeBase.Destruction;
using Assets.CodeBase.Finances;
using Assets.CodeBase.GameStates;
using Assets.CodeBase.Inventory.Items;
using Assets.CodeBase.Player;
using Assets.CodeBase.Player.Respawn;
using Assets.CodeBase.Teams;
using Assets.CodeBase.Vehicles;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

namespace Assets.CodeBase.Inventory
{
	[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
	[UpdateInGroup(typeof(InventorySystemGroup))]
	[UpdateBefore(typeof(InventoryCleanUpSystem))]
	public partial struct InventoryInitializationSystem : ISystem
	{
		public void OnCreate(ref SystemState state) {
			state.RequireForUpdate<BasicInventoryCapacity>();
		}

		public void OnUpdate(ref SystemState state) {
			EntityCommandBuffer ecb = new(Allocator.Temp);

			int basicInventoryCapacity = SystemAPI.GetSingleton<BasicInventoryCapacity>().Value;

			foreach (Entity entity
				in SystemAPI.QueryBuilder()
					.WithAll<InventoryInitializationTag>()
					.Build().ToEntityArray(Allocator.Temp)) {

				NativeArray<InventorySlot> inventory = new(basicInventoryCapacity, Allocator.Persistent);

				for (int i = 0; i < basicInventoryCapacity; i++) {
					ecb.AppendToBuffer(entity, new GhostInventorySlot() { ItemId = InventorySlot.UndefinedItem });

					inventory[i] = new InventorySlot {
						ItemId = InventorySlot.UndefinedItem,
						IsSpawnable = false,
						SpawnedItem = Entity.Null
					};
				}

				ecb.AddComponent(entity, new ItemSlotCollection { Slots = inventory });

				ecb.RemoveComponent<InventoryInitializationTag>(entity);
				ecb.AddComponent<InventoryTag>(entity);
			}

			ecb.Playback(state.EntityManager);
		}
	}

	[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
	[UpdateInGroup(typeof(InventorySystemGroup))]
	[UpdateAfter(typeof(InventoryInitializationSystem))]
	public partial struct SellItemSystem : ISystem
	{
		public void OnCreate(ref SystemState state) {
			state.RequireForUpdate<InGameState>();
			state.RequireForUpdate<ItemRemovalPrefab>();
		}

		public void OnUpdate(ref SystemState state) {
			EntityCommandBuffer ecb = new(Allocator.Temp);

			DynamicBuffer<ItemRemovalPrefab> removalPrefabs = SystemAPI.GetSingletonBuffer<ItemRemovalPrefab>();

			foreach (var (sellRpc, requestSource, requestEntity)
				in SystemAPI.Query<SellItemRpc, ReceiveRpcCommandRequest>()
					.WithEntityAccess()) {

				ecb.DestroyEntity(requestEntity);

				Entity playerEntity = SystemAPI.GetComponent<PlayerEntity>(requestSource.SourceConnection).Value;

				ItemSlotCollection inventory = SystemAPI.GetComponent<ItemSlotCollection>(playerEntity);

				if (sellRpc.Slot >= inventory.Slots.Length ||
				    sellRpc.Slot < 0)
					continue;


				int soldItemId = inventory.Slots[sellRpc.Slot].ItemId;

				if (soldItemId == InventorySlot.UndefinedItem)
					continue;


				Entity spawnedItem = inventory.Slots[sellRpc.Slot].SpawnedItem;

				inventory.Slots[sellRpc.Slot] = new InventorySlot() {
					IsSpawnable = false,
					ItemId = InventorySlot.UndefinedItem,
					SpawnedItem = Entity.Null
				};

				RefRW<MoneyAmount> moneyAmount = SystemAPI.GetComponentRW<MoneyAmount>(playerEntity);
				moneyAmount.ValueRW.Value -= removalPrefabs[soldItemId].SellCost;

				RespawnedEntity respawnedEntity = SystemAPI.GetComponent<RespawnedEntity>(playerEntity);

				if (!state.EntityManager.Exists(respawnedEntity.Value))
					continue;


				Entity removeItemCommand = ecb.Instantiate(removalPrefabs[soldItemId].Item);

				ecb.SetComponent(removeItemCommand, new VehicleWithItem() { Value = respawnedEntity.Value });

				if (state.EntityManager.Exists(spawnedItem))
					ecb.AddComponent<DestroyEntityTag>(spawnedItem);
			}

			ecb.Playback(state.EntityManager);
		}
	}

	[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
	[UpdateInGroup(typeof(InventorySystemGroup))]
	[UpdateAfter(typeof(SellItemSystem))]
	public partial struct BuyItemSystem : ISystem
	{
		public void OnCreate(ref SystemState state) {
			state.RequireForUpdate<InGameState>();
			state.RequireForUpdate<ItemCreationPrefab>();
		}

		public void OnUpdate(ref SystemState state) {
			EntityCommandBuffer ecb = new(Allocator.Temp);

			DynamicBuffer<ItemCreationPrefab> itemBuffer = SystemAPI.GetSingletonBuffer<ItemCreationPrefab>();

			foreach (var (itemRpc, requestSource, requestEntity)
				in SystemAPI.Query<BuyItemRpc, ReceiveRpcCommandRequest>()
					.WithEntityAccess()) {

				ecb.DestroyEntity(requestEntity);

				if (itemRpc.ItemId > itemBuffer.Length ||
				    itemRpc.ItemId < 0)
					continue;


				Entity playerEntity = SystemAPI.GetComponent<PlayerEntity>(requestSource.SourceConnection).Value;

				RefRW<MoneyAmount> playerMoney = SystemAPI.GetComponentRW<MoneyAmount>(playerEntity);

				if (itemBuffer[itemRpc.ItemId].BuyCost > playerMoney.ValueRO.Value)
					continue;


				ItemSlotCollection inventory = SystemAPI.GetComponent<ItemSlotCollection>(playerEntity);

				int freeSlot = -1;
				for (int i = 0; i < inventory.Slots.Length && freeSlot == -1; i++)
					if (inventory.Slots[i].ItemId == InventorySlot.UndefinedItem)
						freeSlot = i;

				if (freeSlot == -1)
					continue;


				playerMoney.ValueRW.Value -= itemBuffer[itemRpc.ItemId].BuyCost;

				bool itemIsSpawnable =
					SystemAPI.HasComponent<SpawnableItemSettings>(itemBuffer[itemRpc.ItemId].Command);

				inventory.Slots[freeSlot] = new InventorySlot {
					ItemId = itemRpc.ItemId,
					IsSpawnable = itemIsSpawnable,
					SpawnedItem = Entity.Null
				};

				Entity respawnedEntity = SystemAPI.GetComponent<RespawnedEntity>(playerEntity).Value;

				if (respawnedEntity == Entity.Null)
					continue;


				Entity addItemCommand = ecb.Instantiate(itemBuffer[itemRpc.ItemId].Command);

				if (!itemIsSpawnable)
					continue;


				ecb.SetComponent(addItemCommand, new VehicleWithItem() { Value = respawnedEntity });
				ecb.SetComponent(addItemCommand, new SpawnableItemSettings {
					PlayerEntity = playerEntity,
					InventorySlot = freeSlot,
					SpawnParent = SystemAPI.GetComponent<VehicleItemSlot>(respawnedEntity).Value,
					ItemTeam = SystemAPI.GetComponent<UnitTeam>(playerEntity).Value
				});
			}

			ecb.Playback(state.EntityManager);
		}
	}

	[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
	[UpdateInGroup(typeof(InventorySystemGroup))]
	[UpdateAfter(typeof(BuyItemSystem))]
	public partial struct RecreateRespawnedVehicleItemsSystem : ISystem
	{
		public void OnCreate(ref SystemState state) {
			state.RequireForUpdate<InGameState>();
			state.RequireForUpdate<ShouldRespawnTag>();
		}

		public void OnUpdate(ref SystemState state) {
			EntityCommandBuffer ecb = new(Allocator.Temp);

			DynamicBuffer<ItemCreationPrefab> itemBuffer = SystemAPI.GetSingletonBuffer<ItemCreationPrefab>();

			foreach (var (team, inventory, respawnedEntity, playerEntity)
				in SystemAPI.Query<UnitTeam, ItemSlotCollection, RespawnedEntity>()
					.WithAll<ShouldRespawnTag>()
					.WithEntityAccess()) {

				for (int i = 0; i < inventory.Slots.Length; i++) {
					if (inventory.Slots[i].ItemId == InventorySlot.UndefinedItem)
						continue;


					if (inventory.Slots[i].IsSpawnable) {
						Entity addItemCommand = ecb.Instantiate(itemBuffer[inventory.Slots[i].ItemId].Command);

						ecb.SetComponent(addItemCommand, new VehicleWithItem() { Value = respawnedEntity.Value });
						ecb.SetComponent(addItemCommand, new SpawnableItemSettings {
							PlayerEntity = playerEntity,
							InventorySlot = i,
							SpawnParent = SystemAPI.GetComponent<VehicleItemSlot>(respawnedEntity.Value).Value,
							ItemTeam = team.Value
						});
					}
				}
			}

			ecb.Playback(state.EntityManager);
		}
	}

	[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
	[UpdateInGroup(typeof(InventorySystemGroup))]
	[UpdateAfter(typeof(RecreateRespawnedVehicleItemsSystem))]
	public partial struct SpawnItemSystem : ISystem
	{
		public void OnCreate(ref SystemState state) {
			state.RequireForUpdate<InGameState>();
		}

		public void OnUpdate(ref SystemState state) {
			EntityCommandBuffer ecb = new(Allocator.Temp);

			foreach (var (vehicle, item, settings, spawnCommand)
				in SystemAPI.Query<VehicleWithItem, SpawnableItem, SpawnableItemSettings>()
					.WithAll<ItemCreationTag>()
					.WithEntityAccess()) {

				Entity instantiatedItem = ecb.Instantiate(item.Value);

				ecb.AddComponent(instantiatedItem, new Parent() { Value = settings.SpawnParent });
				ecb.SetComponent(instantiatedItem, new UnitTeam { Value = settings.ItemTeam });

				ecb.AppendToBuffer(vehicle.Value, new LinkedEntityGroup() { Value = instantiatedItem });

				ecb.SetComponent(spawnCommand, new InstantiatedItem() { Value = instantiatedItem });
			}

			ecb.Playback(state.EntityManager);

			foreach (var (instantiatedItem, settings)
				in SystemAPI.Query<InstantiatedItem, SpawnableItemSettings>()
					.WithAll<ItemCreationTag>()) {

				ItemSlotCollection inventory = SystemAPI.GetComponent<ItemSlotCollection>(settings.PlayerEntity);

				inventory.Slots[settings.InventorySlot] = new InventorySlot {
					ItemId = inventory.Slots[settings.InventorySlot].ItemId,
					IsSpawnable = true,
					SpawnedItem = instantiatedItem.Value,
				};
			}
		}
	}

	[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
	[UpdateInGroup(typeof(InventorySystemGroup))]
	[UpdateAfter(typeof(SpawnItemSystem))]
	public partial struct UpdateGhostInventorySystem : ISystem
	{
		public void OnCreate(ref SystemState state) {
			state.RequireForUpdate<InGameState>();
			state.RequireForUpdate<ItemCommandTag>();
		}

		public void OnUpdate(ref SystemState state) {
			foreach (var (inventory, ghostInventory)
				in SystemAPI.Query<ItemSlotCollection, DynamicBuffer<GhostInventorySlot>>()) {

				ghostInventory.Clear();

				for (int i = 0; i < inventory.Slots.Length; i++) 
					ghostInventory.Add(new GhostInventorySlot() { ItemId = inventory.Slots[i].ItemId });
			}
		}
	}

	[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
	[UpdateInGroup(typeof(InventorySystemGroup))]
	[UpdateAfter(typeof(UpdateGhostInventorySystem))]
	public partial struct DestroyItemCommandEntitySystem : ISystem
	{
		public void OnCreate(ref SystemState state) {
			state.RequireForUpdate<InGameState>();
		}

		public void OnUpdate(ref SystemState state) {
			EntityQuery query = SystemAPI.QueryBuilder()
				.WithAll<ItemCommandTag>().Build();

			state.EntityManager.DestroyEntity(query);
		}
	}

	[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
	[UpdateInGroup(typeof(InventorySystemGroup))]
	[UpdateAfter(typeof(DestroyItemCommandEntitySystem))]
	public partial struct InventoryCleanUpSystem : ISystem
	{
		public void OnUpdate(ref SystemState state) {
			EntityCommandBuffer ecb = new(Allocator.Temp);

			foreach (var (itemCollection, entity)
				in SystemAPI.Query<ItemSlotCollection>()
					.WithNone<InventoryTag>()
					.WithEntityAccess()) {

				itemCollection.Slots.Dispose();

				ecb.RemoveComponent<ItemSlotCollection>(entity);
			}

			ecb.Playback(state.EntityManager);
		}
	}
}