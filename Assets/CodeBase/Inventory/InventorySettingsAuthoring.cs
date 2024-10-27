using Assets.CodeBase.Inventory.Items;
using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Inventory
{
    public class InventorySettingsAuthoring : MonoBehaviour
    {
        [SerializeField] private InventorySettings _inventorySettings;

        public int BasicInventoryCapacity => _inventorySettings.InventorySize;

        public class InventorySettingsBaker : Baker<InventorySettingsAuthoring>
        {
            public override void Bake(InventorySettingsAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new BasicInventoryCapacity { Value =  authoring.BasicInventoryCapacity });
            }
        }
    }
}
