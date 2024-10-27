using UnityEngine;

namespace Assets.CodeBase.Inventory.Items
{
	[CreateAssetMenu(menuName = Constants.ScriptableObjectsNames.ItemDescription)]
	public class ItemDescription : ScriptableObject
	{
		public int Id;

		public string Name;
		public int Cost;

		public bool IsWeapon;
		public GameObject WeaponPrefab;
	}
}