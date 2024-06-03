using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Targeting
{
    public class TargeterAuthoring : MonoBehaviour
    {
        [SerializeField] private float _targetSearchRange = 20;

        public float TargetSearchRange => _targetSearchRange;

        public class TargeterBaker : Baker<TargeterAuthoring>
        {
            public override void Bake(TargeterAuthoring authoring) {
                Entity targeter = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<Targeter>(targeter);
                AddComponent(targeter, new TargeterRange { Value = authoring.TargetSearchRange });
                AddComponent(targeter, new CurrentTarget { Value = Entity.Null });
            }
        }
    }
}