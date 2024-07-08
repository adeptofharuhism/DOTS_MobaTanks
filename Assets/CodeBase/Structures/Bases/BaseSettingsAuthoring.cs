using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.CodeBase.Structures.Bases
{
    public class BaseSettingsAuthoring : MonoBehaviour
    {
        [SerializeField] private Transform _blueBasePosition;
        [SerializeField] private Transform _orangeBasePosition;

        public float3 BlueBasePosition => _blueBasePosition.position;
        public float3 OrangeBasePosition => _orangeBasePosition.position;

        public class BaseSettingsBaker : Baker<BaseSettingsAuthoring>
        {
            public override void Bake(BaseSettingsAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new BaseSpawnPositions {
                    BlueBase = authoring.BlueBasePosition,
                    OrangeBase = authoring.OrangeBasePosition
                });
            }
        }
    }
}
