using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Inventory
{
    public class InventorySettingsAuthoring : MonoBehaviour
    {
        [SerializeField] private int _basicInventoryCapacity;

        public int BasicInventoryCapacity => _basicInventoryCapacity;

        public class InventorySettingsBaker : Baker<InventorySettingsAuthoring>
        {
            public override void Bake(InventorySettingsAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new BasicInventoryCapacity { Value =  authoring.BasicInventoryCapacity });
            }
        }
    }
}
