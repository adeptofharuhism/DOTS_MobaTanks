using Assets.CodeBase.Player;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Inventory
{
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

                ecb.AddComponent(entity, new ItemEntityCollection {
                    Items = new NativeArray<Entity>(basicInventoryCapacity, Allocator.Persistent)
                });

                ecb.RemoveComponent<InventoryInitializationTag>(entity);
                ecb.AddComponent<InventoryTag>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [UpdateInGroup(typeof(InventorySystemGroup))]
    [UpdateAfter(typeof(InventoryInitializationSystem))]
    public partial struct InventoryAddItemSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<BasicInventoryCapacity>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (addItemData, requestSource, requestEntity)
                in SystemAPI.Query<AddItemToInventoryRpc, ReceiveRpcCommandRequest>()
                .WithEntityAccess()) {

                ItemEntityCollection itemEntityCollection =
                    SystemAPI.GetComponent<ItemEntityCollection>(
                        SystemAPI.GetComponent<PlayerEntity>(requestSource.SourceConnection).Value);



                ecb.DestroyEntity(requestEntity);
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [UpdateInGroup(typeof(InventorySystemGroup))]
    [UpdateAfter(typeof(InventoryAddItemSystem))]
    public partial struct InventoryCleanUpSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new(Allocator.Temp);

            foreach (var (itemCollection, entity)
                in SystemAPI.Query<ItemEntityCollection>()
                .WithNone<InventoryTag>()
                .WithEntityAccess()) {

                itemCollection.Items.Dispose();

                ecb.RemoveComponent<ItemEntityCollection>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
