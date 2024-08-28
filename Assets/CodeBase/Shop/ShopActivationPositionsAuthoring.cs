using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.CodeBase.Shop
{
    public class ShopActivationPositionsAuthoring : MonoBehaviour
    {
        [SerializeField] private float _activationDistance;
        [SerializeField] private Transform _blueShopPosition;
        [SerializeField] private Transform _orangeShopPosition;

        public float ActivationDistance => _activationDistance;
        public Transform BlueShopPosition => _blueShopPosition;
        public Transform OrangeShopPosition => _orangeShopPosition;

        public class ShopActivationPositionsBaker : Baker<ShopActivationPositionsAuthoring>
        {
            public override void Bake(ShopActivationPositionsAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new ShopActivationSettings {
                    SquaredActivationDistance = math.square(authoring.ActivationDistance),
                    BlueShopPosition = authoring.BlueShopPosition.position,
                    OrangeShopPosition = authoring.OrangeShopPosition.position
                });
            }
        }
    }
}
