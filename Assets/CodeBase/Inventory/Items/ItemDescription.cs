using UnityEngine;

namespace Assets.CodeBase.Inventory.Items
{
    [CreateAssetMenu(menuName = Constants.ScriptableObjectsNames.ItemDescription)]
    public class ItemDescription : ScriptableObject
    {
        public int ItemId;

        public bool IsWeapon;
        public GameObject WeaponPrefab;
    }
}
