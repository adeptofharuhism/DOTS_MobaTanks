using System;
using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Inventory.Items
{
    public class ItemCollectionSettingsAuthoring : MonoBehaviour
    {
        [SerializeField] private ItemCollection _itemCollection;

        public ItemCollection ItemCollection => _itemCollection;

        public class Baker : Baker<ItemCollectionSettingsAuthoring>
        {
            private const string CreationSuffix = "ForCreation";
            private const string RemovalSuffix = "ForRemoval";

            public override void Bake(ItemCollectionSettingsAuthoring authoring) {
                Entity settings = GetEntity(TransformUsageFlags.None);

                AddComponent(settings, new EmptyItemCommand() { Command = CreateEmptyItemCommand() });

                DynamicBuffer<ItemCreationPrefabElement> creationBuffer = AddBuffer<ItemCreationPrefabElement>(settings);
                DynamicBuffer<ItemRemovalPrefabElement> removalBuffer = AddBuffer<ItemRemovalPrefabElement>(settings);

                foreach (ItemDescription item in authoring.ItemCollection.ItemDescriptions) {
                    creationBuffer.Add(new ItemCreationPrefabElement {
                        Command = MakeItemCreationPrefab(item),
                        BuyCost = item.Cost
                    });

                    removalBuffer.Add(new ItemRemovalPrefabElement {
                        Item = MakeItemRemovalPrefab(item),
                        SellCost = (int)(item.Cost * authoring.ItemCollection.SellMultiplier)
                    });
                }
            }

            private Entity CreateEmptyItemCommand() {
                Entity emptyItemCommand = CreateAdditionalEntity(TransformUsageFlags.None);

                AddComponent<Prefab>(emptyItemCommand);
                AddComponent<ItemCommandTag>(emptyItemCommand);

                return emptyItemCommand;
            }

            private Entity MakeItemCreationPrefab(ItemDescription item) {
                Entity itemCreationEntity =
                    CreateAdditionalEntity(TransformUsageFlags.None, entityName: item.Name + CreationSuffix);

                AddCommonComponents(itemCreationEntity);

                AddComponent<ItemCreationTag>(itemCreationEntity);
                AddComponent<InstantiatedItem>(itemCreationEntity);

                switch (item.ItemType) {
                    case ItemType.VehicleVitality:
                        break;
                    case ItemType.Weapon:
                        AddComponent<SpawnableItemSettings>(itemCreationEntity);
                        AddComponent(itemCreationEntity, new SpawnableItem {
                            Value = GetEntity(item.WeaponPrefab, TransformUsageFlags.Dynamic)
                        });
                        break;
                    case ItemType.SpawnableUnit:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return itemCreationEntity;
            }

            private Entity MakeItemRemovalPrefab(ItemDescription item) {
                Entity itemRemovalEntity =
                    CreateAdditionalEntity(TransformUsageFlags.None, entityName: item.Name + RemovalSuffix);

                AddCommonComponents(itemRemovalEntity);

                AddComponent<ItemRemovalPrefabElement>(itemRemovalEntity);

                return itemRemovalEntity;
            }

            private void AddCommonComponents(Entity entity) {
                AddComponent<Prefab>(entity);
                AddComponent<ItemCommandTag>(entity);
                AddComponent<VehicleWithItem>(entity);
            }
        }
    }
}