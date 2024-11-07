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

		private SerializedProperty _itemImage;
		private SerializedProperty _weaponPrefab;
		private SerializedProperty _itemType;

		private void OnEnable() {
			ItemDescription target = (ItemDescription)this.target;

			_itemImage = serializedObject.FindProperty(nameof(target.Image));
			_weaponPrefab = serializedObject.FindProperty(nameof(target.WeaponPrefab));
			_itemType = serializedObject.FindProperty(nameof(target.ItemType));
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

			string updatedName = GUILayout.TextField(target.Name);
			target.Name = updatedName;

			int updatedCost = EditorGUILayout.IntField(new GUIContent(nameof(target.Cost)), target.Cost);
			target.Cost = updatedCost;

			EditorGUILayout.PropertyField(_itemImage, new GUIContent(nameof(target.Image)));

			EditorGUILayout.PropertyField(_itemType, new GUIContent(nameof(target.ItemType)));
		}

		private void DrawWeaponSection(ItemDescription target) {
			GUILayout.Space(SpacePixels);
			GUILayout.Label(WeaponSettingsLabelText);

			EditorGUILayout.PropertyField(_weaponPrefab, new GUIContent(nameof(target.WeaponPrefab)));
		}
	}
}