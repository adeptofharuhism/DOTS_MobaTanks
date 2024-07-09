using Assets.CodeBase.Combat.Teams;
using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Weapon.WeaponGroup
{
    [RequireComponent(typeof(TeamAuthoring))]
    public class WeaponGroupAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject _weaponContainer;
        [SerializeField] private List<WeaponGroupElement> _weapons;

        public GameObject WeaponContainer => _weaponContainer;
        public List<WeaponGroupElement> Weapons => _weapons;

        public class WeaponGroupBaker : Baker<WeaponGroupAuthoring>
        {
            public override void Bake(WeaponGroupAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<ShouldInitializeWeaponGroup>(entity);
                AddComponent(entity, new WeaponContainer {
                    Value = GetEntity(authoring.WeaponContainer, TransformUsageFlags.Dynamic)
                });

                DynamicBuffer<WeaponBufferElement> weaponBuffer = AddBuffer<WeaponBufferElement>(entity);
                foreach (WeaponGroupElement weaponGroupElement in authoring.Weapons)
                    for (int i = 0; i < weaponGroupElement.Amount; i++)
                        weaponBuffer.Add(new WeaponBufferElement {
                            WeaponPrefab = GetEntity(weaponGroupElement.WeaponPrefab, TransformUsageFlags.Dynamic)
                        });
            }
        }

        [Serializable]
        public class WeaponGroupElement
        {
            public GameObject WeaponPrefab;
            public int Amount;
        }
    }
}
