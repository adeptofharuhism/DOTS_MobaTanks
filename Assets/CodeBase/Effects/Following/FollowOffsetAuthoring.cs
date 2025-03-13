using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Effects.Following
{
    public class FollowOffsetAuthoring : MonoBehaviour
    {
        [SerializeField] private Vector3 _followingOffset;

        public Vector3 FollowingOffset => _followingOffset;

        private class Baker : Baker<FollowOffsetAuthoring>
        {
            public override void Bake(FollowOffsetAuthoring authoring) {
                Entity follower = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(follower, new FollowOffset {
                    Value = authoring.FollowingOffset
                });
            }
        }
    }
}
