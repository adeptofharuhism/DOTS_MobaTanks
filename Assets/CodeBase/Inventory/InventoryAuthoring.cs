using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Inventory
{
    public class InventoryAuthoring : MonoBehaviour
    {
        public class InventoryBaker : Baker<InventoryAuthoring>
        {
            public override void Bake(InventoryAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent<InventoryInitializationTag>(entity);
            }
        }
    }
}
