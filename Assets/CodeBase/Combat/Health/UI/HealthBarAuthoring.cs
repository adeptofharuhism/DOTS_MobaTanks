using Assets.CodeBase.Teams;
using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Combat.Health.UI
{
    public struct HealthBarInitializeTag : IComponentData { }

    public struct HealthBarParts : IComponentData
    {
        public Entity Fill;
        public Entity Outline;
    }
    
    public class HealthBarAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject _fill;
        [SerializeField] private GameObject _outline;

        public GameObject Fill => _fill;
        public GameObject Outline => _outline;

        private class Baker : Baker<HealthBarAuthoring>
        {
            public override void Bake(HealthBarAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<HealthBarInitializeTag>(entity);
                AddComponent<UnitTeam>(entity);

                AddComponent(
                    entity,
                    new HealthBarParts()
                    {
                        Fill = GetEntity(authoring.Fill, TransformUsageFlags.Dynamic),
                        Outline = GetEntity(authoring.Outline, TransformUsageFlags.Dynamic)
                    });
            }
        }
    }
}