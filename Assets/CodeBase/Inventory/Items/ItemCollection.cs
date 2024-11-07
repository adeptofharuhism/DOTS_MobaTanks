using System.Collections.Generic;
using UnityEngine;

namespace Assets.CodeBase.Inventory.Items
{
    [CreateAssetMenu(menuName = Constants.ScriptableObjectsNames.ItemCollection)]
    public class ItemCollection : ScriptableObject
    {
        [Range(0, 1)] public float SellMultiplier;

        public ItemType[] ItemTypes;
        
        public List<ItemDescription> ItemDescriptions;
    }
}
