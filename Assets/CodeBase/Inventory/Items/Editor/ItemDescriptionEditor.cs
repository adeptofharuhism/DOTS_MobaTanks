using UnityEditor;
using UnityEngine;

namespace Assets.CodeBase.Inventory.Items
{
    [CustomEditor(typeof(ItemDescription))]
    public class ItemDescriptionEditor : Editor
    {
        private const string WeaponSettingsLabelText = "Weapon Settings";

        private SerializedProperty _weaponPrefab;

        private void OnEnable() {
            ItemDescription target = (ItemDescription)this.target;

            _weaponPrefab = serializedObject.FindProperty(nameof(target.WeaponPrefab));
        }

        public override void OnInspectorGUI() {
            ItemDescription target = (ItemDescription)this.target;

            DrawItemName(target);
            DrawWeaponSection(target);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawItemName(ItemDescription target) {
            GUILayout.Label(nameof(target.Name));

            string updatedName = GUILayout.TextField(target.Name);
            target.Name = updatedName;
        }

        private void DrawWeaponSection(ItemDescription target) {
            GUILayout.Label(WeaponSettingsLabelText);

            bool isWeaponFlag = GUILayout.Toggle(target.IsWeapon, nameof(target.IsWeapon));
            target.IsWeapon = isWeaponFlag;

            if (isWeaponFlag)
                EditorGUILayout.PropertyField(_weaponPrefab, new GUIContent(nameof(target.WeaponPrefab)));
        }
    }
}
