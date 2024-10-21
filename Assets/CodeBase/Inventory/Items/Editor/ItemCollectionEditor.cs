using UnityEditor;
using UnityEngine;

namespace Assets.CodeBase.Inventory.Items
{
    [CustomEditor(typeof(ItemCollection))]
    public class ItemCollectionEditor : Editor
    {
        private const string EnumerateItemsButtonText = "Enumerate items";

        public override void OnInspectorGUI() {
            if (GUILayout.Button(EnumerateItemsButtonText))
                EnumerateItems();

            DrawDefaultInspector();
        }

        private void EnumerateItems() {
            ItemCollection items = (ItemCollection)target;

            if (items.ItemDescriptions.Count == 0) {
                UnityEngine.Debug.Log($"Found no items to enumerate inside Item Collection");
                return;
            }

            int successfulEnumerations = 0;
            for (int i = 0; i < items.ItemDescriptions.Count; i++)
                if (items.ItemDescriptions[i] != null)
                    items.ItemDescriptions[i].ItemId = successfulEnumerations++;

            UnityEngine.Debug.Log($"Enumerated {successfulEnumerations} items inside Item Collection");
        }
    }
}
