using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Assets.CodeBase.Relevancy
{
    [RequireComponent(typeof(GhostAuthoringComponent))]
    public class RelevancyAuthoring : MonoBehaviour
    {
        public class RelevancyBaker : Baker<RelevancyAuthoring>
        {
            public override void Bake(RelevancyAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent<OwnerRelevancyTag>(entity);
            }
        }
    }
}
