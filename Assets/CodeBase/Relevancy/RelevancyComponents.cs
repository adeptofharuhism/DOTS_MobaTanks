using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Relevancy
{
    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct OwnerRelevancyTag : IComponentData { }
}
