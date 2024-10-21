using System.Collections.Generic;
using UnityEngine;

namespace Assets.CodeBase.Inventory.Items
{
    [CreateAssetMenu(menuName = Constants.ScriptableObjectsNames.ItemCollection)]
    public class ItemCollection : ScriptableObject
    {
        public List<ItemDescription> ItemDescriptions;
    }
}
