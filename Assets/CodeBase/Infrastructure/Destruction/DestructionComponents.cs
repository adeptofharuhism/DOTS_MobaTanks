using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Infrastructure.Destruction
{
    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public partial struct DestroyEntityTag : IComponentData { }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public partial struct SelfDestructTimeLeft : IComponentData
    {
        public float Value;
    }

    public partial struct ClientSelfDestructTimeLeft : IComponentData
    {
        public float Value;
    }
}
