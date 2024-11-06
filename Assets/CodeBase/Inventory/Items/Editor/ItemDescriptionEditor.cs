using Cinemachine.Editor;
using UnityEditor;
using UnityEngine;

namespace Assets.CodeBase.Inventory.Items
{
    [CustomEditor(typeof(ItemDescription))]
    public class ItemDescriptionEditor : Editor
    {
        private const int SpacePixels = 6;
        private const string WeaponSettingsLabelText = "Weapon Settings";

        private SerializedProperty _itemImage;
        private SerializedProperty _weaponPrefab;

        private void OnEnable() {
            ItemDescription target = (ItemDescription)this.target;

            _itemImage = serializedObject.FindProperty(nameof(target.Image));
            _weaponPrefab = serializedObject.FindProperty(nameof(target.WeaponPrefab));
        }

        public override void OnInspectorGUI() {
            ItemDescription target = (ItemDescription)this.target;

            DrawCommonOptions(target);
            DrawWeaponSection(target);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawCommonOptions(ItemDescription target) {
            GUILayout.Label(nameof(target.Name));

            string updatedName = GUILayout.TextField(target.Name);
            target.Name = updatedName;

            int updatedCost = EditorGUILayout.IntField(new GUIContent(nameof(target.Cost)), target.Cost);
            target.Cost = updatedCost;

            EditorGUILayout.PropertyField(_itemImage, new GUIContent(nameof(target.Image)));
        }

        private void DrawWeaponSection(ItemDescription target) {
            GUILayout.Space(SpacePixels);
            GUILayout.Label(WeaponSettingsLabelText);

            bool isWeaponFlag = GUILayout.Toggle(target.IsWeapon, nameof(target.IsWeapon));
            target.IsWeapon = isWeaponFlag;

            if (isWeaponFlag)
                EditorGUILayout.PropertyField(_weaponPrefab, new GUIContent(nameof(target.WeaponPrefab)));
        }
    }
}
