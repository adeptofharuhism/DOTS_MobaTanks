using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Effects.Following
{
    public class FollowTargetAuthoring : MonoBehaviour
    {
        private class Baker : Baker<FollowTargetAuthoring>
        {
            public override void Bake(FollowTargetAuthoring authoring) {
                Entity follower = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<FollowTarget>(follower);
            }
        }
    }
}
