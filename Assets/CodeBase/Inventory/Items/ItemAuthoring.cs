using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Inventory.Items
{
    public class ItemAuthoring : MonoBehaviour
    {
        public class ItemBaker : Baker<ItemAuthoring>
        {
            public override void Bake(ItemAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.None);
            }
        }
    }
}
