using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Effects.Following
{
    [RequireComponent(typeof(FollowTargetAuthoring))]
    public class LinkToTargetAuthoring : MonoBehaviour
    {
        private class Baker : Baker<LinkToTargetAuthoring>
        {
            public override void Bake(LinkToTargetAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<LinkToTarget>(entity);
            }
        }
    }
}