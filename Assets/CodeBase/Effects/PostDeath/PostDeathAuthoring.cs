using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Effects.PostDeath
{
    public class PostDeathAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject _postDeathEffect;

        public GameObject PostDeathEffect => _postDeathEffect;

        public class PostDeathBaker : Baker<PostDeathAuthoring>
        {
            public override void Bake(PostDeathAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new PostDeathEffectInitializationData {
                    EffectPfefab = GetEntity(authoring.PostDeathEffect, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}
