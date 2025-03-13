using Assets.CodeBase.Inventory.Items;
using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Inventory.TargetingRange
{
    public class TargetingRangeSettingsAuthoring : MonoBehaviour
    {
        [SerializeField] private ItemCollection _itemCollection;
        [SerializeField] private GameObject _closeRangeDecalObject;
        [SerializeField] private GameObject _farRangeDecalObject;

        public ItemCollection ItemCollection => _itemCollection;
        public GameObject CloseRangeDecalObject => _closeRangeDecalObject;
        public GameObject FarRangeDecalObject => _farRangeDecalObject;

        private class Baker : Baker<TargetingRangeSettingsAuthoring>
        {
            public override void Bake(TargetingRangeSettingsAuthoring authoring) {
                Entity settings = GetEntity(TransformUsageFlags.None);

                AddComponent(settings, new TargetingDecalPrefabs() {
                    CloseRangeDecalPrefab = GetEntity(authoring.CloseRangeDecalObject, TransformUsageFlags.Dynamic),
                    FarRangeDecalPrefab = GetEntity(authoring.FarRangeDecalObject, TransformUsageFlags.Dynamic)
                });

                DynamicBuffer<ItemInfoElement> clientSideInfo = AddBuffer<ItemInfoElement>(settings);
                DynamicBuffer<UpdateTargetRangeElement> updateTargetRangeBuffer =
                    AddBuffer<UpdateTargetRangeElement>(settings);

                foreach (ItemDescription item in authoring.ItemCollection.ItemDescriptions) {
                    clientSideInfo.Add(new ItemInfoElement {
                        TargetRange =
                            item.ItemType == ItemType.Weapon
                                ? item.TargetingRange
                                : 0
                    });
                }

                //Basic weapon target range. Stays there unchanged until inventory and basic weapon rework.
                updateTargetRangeBuffer.Add(new UpdateTargetRangeElement {
                    SlotId = 0,
                    TargetRange = 40
                });
            }
        }
    }
}