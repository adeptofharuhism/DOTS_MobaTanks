using Unity.Entities;

namespace Assets.CodeBase.Weapon.WeaponGroup
{
    public struct ShouldInitializeWeaponGroup : IComponentData { }

    public struct WeaponGroupSlot : IComponentData
    {
        public Entity Value;
    }

    public struct WeaponBufferElement : IBufferElementData
    {
        public Entity WeaponPrefab;
    }
}
