using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Inventory.Items
{
    public class ItemCollectionSettingsAuthoring : MonoBehaviour
    {
        [SerializeField] private ItemCollection _itemCollection;
        [SerializeField, Range(0, 1)] 
        private float _sellCostMultiplier = .5f;

        public ItemCollection ItemCollection => _itemCollection;
        public float SellCostMultiplier => _sellCostMultiplier;

        public class ItemCollectionSettingsBaker : Baker<ItemCollectionSettingsAuthoring>
        {
            private const string CreationSuffix = "ForCreation";
            private const string RemovalSuffix = "ForRemoval";

            public override void Bake(ItemCollectionSettingsAuthoring authoring) {
                Entity settings = GetEntity(TransformUsageFlags.None);

                DynamicBuffer<ItemCreationPrefab> creationBuffer = AddBuffer<ItemCreationPrefab>(settings);
                DynamicBuffer<ItemRemovalPrefab> removalBuffer = AddBuffer<ItemRemovalPrefab>(settings);

                foreach (ItemDescription item in authoring.ItemCollection.ItemDescriptions) {
                    creationBuffer.Add(new ItemCreationPrefab {
                        Item = MakeItemCreationPrefab(item),
                        BuyCost = item.Cost
                    });

                    removalBuffer.Add(new ItemRemovalPrefab {
                        Item = MakeItemRemovalPrefab(item),
                        SellCost = (int)(item.Cost * authoring.SellCostMultiplier)
                    });
                }
            }

            private Entity MakeItemCreationPrefab(ItemDescription item) {
                Entity itemCreationEntity = CreateAdditionalEntity(TransformUsageFlags.None, entityName: item.Name + CreationSuffix);

                AddComponent<Prefab>(itemCreationEntity);
                AddComponent<ItemCreationTag>(itemCreationEntity);

                return itemCreationEntity;
            }

            private Entity MakeItemRemovalPrefab(ItemDescription item) {
                Entity itemRemovalEntity = CreateAdditionalEntity(TransformUsageFlags.None, entityName: item.Name + RemovalSuffix);

                AddComponent<Prefab>(itemRemovalEntity);
                AddComponent<ItemRemovalPrefab>(itemRemovalEntity);

                return itemRemovalEntity;
            }
        }
    }
}
