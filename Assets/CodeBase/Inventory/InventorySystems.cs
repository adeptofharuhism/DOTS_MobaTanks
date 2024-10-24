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
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            int basicInventoryCapacity = SystemAPI.GetSingleton<BasicInventoryCapacity>().Value;

            foreach (Entity entity
                in SystemAPI.QueryBuilder()
                .WithAll<InventoryInitializationTag>()
                .Build().ToEntityArray(Allocator.Temp)) {

                NativeArray<InventorySlot> inventory = new NativeArray<InventorySlot>(basicInventoryCapacity, Allocator.Persistent);
                for (int i = 0; i < basicInventoryCapacity; i++)
                    inventory[i] = new InventorySlot {
                        ItemId = InventorySlot.UndefinedItem,
                        SpawnedItem = Entity.Null
                    };
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
    public partial struct BuyItemSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
            state.RequireForUpdate<ItemCreationPrefab>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            DynamicBuffer<ItemCreationPrefab> itemBuffer = SystemAPI.GetSingletonBuffer<ItemCreationPrefab>();

            foreach (var (itemRpc, requestSource, requestEntity)
                in SystemAPI.Query<BuyItemRpc, ReceiveRpcCommandRequest>()
                .WithEntityAccess()) {

                ecb.DestroyEntity(requestEntity);

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

                inventory.Slots[freeSlot] = new InventorySlot {
                    ItemId = itemRpc.ItemId,
                    SpawnedItem = Entity.Null
                };

                Entity respawnedEntity = SystemAPI.GetComponent<RespawnedEntity>(playerEntity).Value;

                if (respawnedEntity == Entity.Null)
                    continue;


                Entity addItemCommand = ecb.Instantiate(itemBuffer[itemRpc.ItemId].Command);

                if (!SystemAPI.HasComponent<SpawnableItemSettings>(itemBuffer[itemRpc.ItemId].Command))
                    continue;


                ecb.SetComponent(addItemCommand, new SpawnableItemSettings {
                    PlayerEntity = playerEntity,
                    Vehicle = respawnedEntity,
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

                    if (SystemAPI.HasComponent<SpawnableItem>(itemBuffer[inventory.Slots[i].ItemId].Command)) {
                        Entity addItemCommand = ecb.Instantiate(itemBuffer[inventory.Slots[i].ItemId].Command);

                        ecb.SetComponent(addItemCommand, new SpawnableItemSettings {
                            PlayerEntity = playerEntity,
                            Vehicle = respawnedEntity.Value,
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

            foreach (var (item, settings)
                in SystemAPI.Query<SpawnableItem, SpawnableItemSettings>()
                .WithAll<ItemCreationTag>()) {

                Entity instantiatedItem = ecb.Instantiate(item.Value);

                ecb.AddComponent(instantiatedItem, new Parent { Value = settings.SpawnParent });
                ecb.SetComponent(instantiatedItem, new UnitTeam { Value = settings.ItemTeam });
                ecb.AppendToBuffer(settings.Vehicle, new LinkedEntityGroup { Value = instantiatedItem });

                ItemSlotCollection inventory = SystemAPI.GetComponent<ItemSlotCollection>(settings.PlayerEntity);

                inventory.Slots[settings.InventorySlot] = new InventorySlot {
                    ItemId = inventory.Slots[settings.InventorySlot].ItemId,
                    SpawnedItem = instantiatedItem,
                };
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(InventorySystemGroup))]
    [UpdateAfter(typeof(SpawnItemSystem))]
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
