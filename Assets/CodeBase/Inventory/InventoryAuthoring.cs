using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Inventory
{
    public class InventoryAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject _inventorySlot;

        public GameObject InventorySlot => _inventorySlot;

        public class InventoryBaker : Baker<InventoryAuthoring>
        {
            public override void Bake(InventoryAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent<InventoryInitializationTag>(entity);
                AddComponent(entity, new InventorySlotEntity {
                    Value = GetEntity(authoring.InventorySlot, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}
