using Assets.CodeBase.Constants;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.CodeBase.Inventory.Items
{
    [CreateAssetMenu(menuName = ScriptableObjectsNames.ItemCollection)]
    public class ItemCollection : ScriptableObject
    {
        [Range(0, 1)] public float SellMultiplier;

        public List<GroupedByTypeItemDescriptions> GroupedByTypeItemDescriptions;
        
        public List<ItemDescription> ItemDescriptions;
    }
    
    [Serializable]
    public struct GroupedByTypeItemDescriptions
    {
        public ItemType ItemType;
        public string ItemTypeName;
        public List<ItemDescription> ItemDescriptions;
    }
}
