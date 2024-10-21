using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Inventory.Items
{
    public class ItemCollectionSettingsAuthoring : MonoBehaviour
    {
        [SerializeField] private ItemCollection _itemCollection;

        public ItemCollection ItemCollection => _itemCollection;

        public class ItemCollectionSettingsBaker : Baker<ItemCollectionSettingsAuthoring>
        {
            public override void Bake(ItemCollectionSettingsAuthoring authoring) {
                Entity settings = GetEntity(TransformUsageFlags.None);

                DynamicBuffer<Item> itemBuffer = AddBuffer<Item>(settings);
                foreach (ItemDescription item in authoring.ItemCollection.ItemDescriptions)
                    if (item.IsWeapon)
                        itemBuffer.Add(
                            new Item { Weapon = GetEntity(item.WeaponPrefab, TransformUsageFlags.Dynamic) });
            }
        }
    }
}
