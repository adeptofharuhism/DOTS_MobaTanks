using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.CodeBase.Inventory.Items
{
	[CustomEditor(typeof(ItemCollection))]
	public class ItemCollectionEditor : Editor
	{
		private const string EnumerateItemsButtonText = "Enumerate items";

		private SerializedProperty _itemDescriptions;

		private void OnEnable() {
			ItemCollection target = (ItemCollection)this.target;

			_itemDescriptions = serializedObject.FindProperty(nameof(target.ItemDescriptions));
		}

		public override void OnInspectorGUI() {
			ItemCollection target = (ItemCollection)this.target;

			if (GUILayout.Button(EnumerateItemsButtonText))
				EnumerateItems();

			EditorGUILayout.PropertyField(_itemDescriptions, new GUIContent(nameof(target.ItemDescriptions)));

			serializedObject.ApplyModifiedProperties();
		}

		private void EnumerateItems() {
			ItemCollection items = (ItemCollection)target;

			if (items.ItemDescriptions.Count == 0) {
				UnityEngine.Debug.Log($"Found no items to enumerate inside Item Collection");

				return;
			}

			ItemType[] itemTypes = (ItemType[])Enum.GetValues(typeof(ItemType));

			UnityEngine.Debug.Log($"Parsed {itemTypes.Length} different item types");

			Dictionary<ItemType, int> indexesOfItemTypeGroup = new();
			List<GroupedByTypeItemDescriptions> groupedItemDescriptions = new();

			for (int i = 0; i < itemTypes.Length; i++) {
				indexesOfItemTypeGroup[itemTypes[i]] = i;

				groupedItemDescriptions.Add(new GroupedByTypeItemDescriptions() {
					ItemType = itemTypes[i],
					ItemTypeName = itemTypes[i].ToString(),
					ItemDescriptions = new List<ItemDescription>()
				});
			}

			int successfulEnumerations = 0;

			for (int i = 0; i < items.ItemDescriptions.Count; i++)
				if (items.ItemDescriptions[i] != null) {
					ItemDescription itemDescription = items.ItemDescriptions[i];

					int groupIndexOfDescription = indexesOfItemTypeGroup[itemDescription.ItemType];

					List<ItemDescription> descriptionGroup =
						groupedItemDescriptions[groupIndexOfDescription].ItemDescriptions;

					descriptionGroup.Add(items.ItemDescriptions[i]);

					items.ItemDescriptions[i].Id = successfulEnumerations++;
				}

			foreach (GroupedByTypeItemDescriptions itemGroup in groupedItemDescriptions)
				itemGroup.ItemDescriptions.Sort((x, y) => x.Cost > y.Cost ? 1 : -1);

			items.GroupedByTypeItemDescriptions = groupedItemDescriptions;

			UnityEngine.Debug.Log($"Enumerated {successfulEnumerations} items inside Item Collection");

		}
	}
}