using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Targeting
{
    [RequireComponent(typeof(TargetableAuthoring))]
    public class TargetPositionAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject _specialTargetingPoint;

        public GameObject SpecialTargetingPoint => _specialTargetingPoint;

        public class TargetPositionBaker : Baker<TargetPositionAuthoring>
        {
            public override void Bake(TargetPositionAuthoring authoring) {
                Entity targetable = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(targetable, new TargetPosition {
                    Value = GetEntity(authoring.SpecialTargetingPoint, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}