using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Combat.Health
{
    public class HealthAuthoring : MonoBehaviour
    {
        [SerializeField] private int _maximalHealth;

        public int MaximalHealth => _maximalHealth;

        public class HealthBaker : Baker<HealthAuthoring>
        {
            public override void Bake(HealthAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new MaximalHealthPoints { Value = authoring.MaximalHealth });
                AddComponent(entity, new CurrentHealthPoints { Value = authoring.MaximalHealth });
                AddComponent(entity, new DamageThisFrame { Value = 0 });
                AddBuffer<DamageBufferElement>(entity);
            }
        }
    }
}
