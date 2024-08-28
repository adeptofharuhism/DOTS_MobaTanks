using Unity.Entities;
using Unity.Mathematics;

namespace Assets.CodeBase.Shop
{
    public struct ShopActivationSettings : IComponentData
    {
        public float SquaredActivationDistance;
        public float3 BlueShopPosition;
        public float3 OrangeShopPosition;
    }

    public struct ShopIsActive : IComponentData
    {
        public bool Value;
    }

    public struct SquaredDistanceToShop : IComponentData
    {
        public float Value;
    }

    public struct SquaredShopActivationDistance : IComponentData
    {
        public float Value;
    }

    public struct ShopPosition : IComponentData
    {
        public float3 Value;
    }
}
