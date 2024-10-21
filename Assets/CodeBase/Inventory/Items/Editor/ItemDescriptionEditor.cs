using UnityEditor;
using UnityEngine;

namespace Assets.CodeBase.Inventory.Items
{
    [CustomEditor(typeof(ItemDescription))]
    public class ItemDescriptionEditor : Editor
    {
        private SerializedProperty _weaponPrefab;

        private void OnEnable() {
            ItemDescription target = (ItemDescription)this.target;

            _weaponPrefab = serializedObject.FindProperty(nameof(target.WeaponPrefab));
        }

        public override void OnInspectorGUI() {
            ItemDescription target = (ItemDescription)this.target;

            DrawWeaponSection(target);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawWeaponSection(ItemDescription target) {
            bool isWeaponFlag = GUILayout.Toggle(target.IsWeapon, nameof(target.IsWeapon));
            target.IsWeapon = isWeaponFlag;
            
            if (isWeaponFlag)
                EditorGUILayout.PropertyField(_weaponPrefab, new GUIContent(nameof(target.WeaponPrefab)));
        }
    }
}
