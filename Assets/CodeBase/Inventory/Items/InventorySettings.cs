using UnityEngine;

namespace Assets.CodeBase.Inventory.Items
{
	[CreateAssetMenu(menuName = Constants.ScriptableObjectsNames.InventorySettings)]
	public class InventorySettings : ScriptableObject
	{
		public int InventorySize;
	}
}