using Unity.Entities;
using Unity.NetCode.Hybrid;
using UnityEngine;

namespace Assets.CodeBase.Combat.Health
{
    public class HealthAuthoring : MonoBehaviour
    {
        [Header("Health params")] [SerializeField]
        private int _maximalHealth;

        [Header("View params")] [SerializeField]
        private Vector3 _healthBarOffset;

        public int MaximalHealth => _maximalHealth;

        public Vector3 HealthBarOffset => _healthBarOffset;

        private class Baker : Baker<HealthAuthoring>
        {
            public override void Bake(HealthAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new MaximalHealthPoints { Value = authoring.MaximalHealth });
                AddComponent(entity, new CurrentHealthPoints { Value = authoring.MaximalHealth });
                AddComponent(entity, new DamageThisFrame { Value = 0 });
                AddBuffer<DamageBufferElement>(entity);

                AddComponent<HealthInitializationTag>(entity);
                AddComponent<ReferenceToHealthBarFillArea>(entity);
                AddComponent(entity, new HealthBarOffset { Value = authoring.HealthBarOffset });
            }
        }
    }
}