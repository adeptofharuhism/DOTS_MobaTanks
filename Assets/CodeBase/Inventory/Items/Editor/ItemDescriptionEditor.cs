using Assets.CodeBase.Targeting;
using Assets.CodeBase.Weapon;
using Cinemachine.Editor;
using System;
using UnityEditor;
using UnityEngine;

namespace Assets.CodeBase.Inventory.Items
{
    [CustomEditor(typeof(ItemDescription))]
    public class ItemDescriptionEditor : Editor
    {
        private const int SpacePixels = 6;
        private const string WeaponSettingsLabelText = "Weapon Settings";

        private SerializedProperty _itemName;
        private SerializedProperty _itemCost;
        private SerializedProperty _itemImage;
        private SerializedProperty _itemType;
        private SerializedProperty _weaponPrefab;
        private SerializedProperty _weaponTargetingRange;

        private void OnEnable() {
            ItemDescription target = (ItemDescription)this.target;

            _itemName = serializedObject.FindProperty(nameof(target.Name));
            _itemCost = serializedObject.FindProperty(nameof(target.Cost));
            _itemImage = serializedObject.FindProperty(nameof(target.Image));
            _itemType = serializedObject.FindProperty(nameof(target.ItemType));
            _weaponPrefab = serializedObject.FindProperty(nameof(target.WeaponPrefab));
            _weaponTargetingRange = serializedObject.FindProperty(nameof(target.TargetingRange));
        }

        public override void OnInspectorGUI() {
            ItemDescription target = (ItemDescription)this.target;

            DrawCommonOptions(target);

            switch (target.ItemType) {
                case ItemType.VehicleVitality:
                    break;
                case ItemType.Weapon:
                    DrawWeaponSection(target);
                    break;
                case ItemType.SpawnableUnit:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawCommonOptions(ItemDescription target) {
            GUILayout.Label(nameof(target.Name));

            EditorGUILayout.PropertyField(_itemName, new GUIContent(nameof(target.Name)));

            EditorGUILayout.PropertyField(_itemCost, new GUIContent(nameof(target.Cost)));

            EditorGUILayout.PropertyField(_itemImage, new GUIContent(nameof(target.Image)));

            EditorGUILayout.PropertyField(_itemType, new GUIContent(nameof(target.ItemType)));
        }

        private void DrawWeaponSection(ItemDescription target) {
            GUILayout.Space(SpacePixels);
            GUILayout.Label(WeaponSettingsLabelText);

            EditorGUILayout.PropertyField(_weaponPrefab, new GUIContent(nameof(target.WeaponPrefab)));

            string weaponTargetSearchLabel;
            try {
                GameObject weaponObject = (GameObject)_weaponPrefab.objectReferenceValue;
                TargeterAuthoring targeterAuthoring = weaponObject.GetComponent<TargeterAuthoring>();
                _weaponTargetingRange.floatValue = targeterAuthoring.TargetSearchRange;

                weaponTargetSearchLabel = targeterAuthoring.TargetSearchRange.ToString();
            } catch (Exception) {
                weaponTargetSearchLabel = "Unable to access Targeting Range value";
            }

            EditorGUILayout.LabelField(nameof(target.TargetingRange), weaponTargetSearchLabel);
        }
    }
}