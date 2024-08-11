using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Infrastructure.Destruction
{
    public class SelfDestructionAuthoring : MonoBehaviour
    {
        [SerializeField] private bool _clientOnly = false;
        [SerializeField] private float _lifetime = 1f;

        public bool ClientOnly => _clientOnly;
        public float Lifetime => _lifetime;

        public class SelfDestructionBaker : Baker<SelfDestructionAuthoring>
        {
            public override void Bake(SelfDestructionAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                if (authoring.ClientOnly)
                    AddComponent(entity, new ClientSelfDestructTimeLeft { Value = authoring.Lifetime });
                else
                    AddComponent(entity, new SelfDestructTimeLeft { Value = authoring.Lifetime });
            }
        }
    }
}
