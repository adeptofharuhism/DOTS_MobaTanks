using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Infrastructure.Destruction
{
    public class SelfDestructionAuthoring : MonoBehaviour
    {
        [SerializeField] private float _lifetime = 1f;

        public float Lifetime => _lifetime;

        public class SelfDestructionBaker : Baker<SelfDestructionAuthoring>
        {
            public override void Bake(SelfDestructionAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new SelfDestructCurrentTime { Value = 0 });
                AddComponent(entity, new SelfDestructLifetime { Value = authoring.Lifetime });
            }
        }
    }
}
