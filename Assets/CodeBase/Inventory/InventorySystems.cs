using Assets.CodeBase.Inventory.Items;
using Assets.CodeBase.Player;
using Assets.CodeBase.Player.Respawn;
using Assets.CodeBase.Vehicles;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

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
                        ItemId = -1,
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
    public partial struct InventoryAddItemSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<BasicInventoryCapacity>();
            state.RequireForUpdate<ItemCreationPrefab>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            DynamicBuffer<ItemCreationPrefab> itemBuffer = SystemAPI.GetSingletonBuffer<ItemCreationPrefab>();

            foreach (var (itemRpc, requestSource, requestEntity)
                in SystemAPI.Query<BuyItemRpc, ReceiveRpcCommandRequest>()
                .WithEntityAccess()) {

                UnityEngine.Debug.Log($"Command to add item {itemRpc.ItemId}");

                ecb.DestroyEntity(requestEntity);
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(InventorySystemGroup))]
    [UpdateAfter(typeof(InventoryAddItemSystem))]
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
