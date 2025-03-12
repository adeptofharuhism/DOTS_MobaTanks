using UnityEngine;

namespace Assets.CodeBase.Inventory.Items
{
	[CreateAssetMenu(menuName = Constants.ScriptableObjectsNames.ItemDescription)]
	public class ItemDescription : ScriptableObject
	{
		public int Id;

		public string Name;
		public int Cost;
		public Texture2D Image;
		public ItemType ItemType;

		public GameObject WeaponPrefab;
		public float TargetingRange;
	}

	public enum ItemType
	{
		VehicleVitality,
		Weapon,
		SpawnableUnit
	}
}