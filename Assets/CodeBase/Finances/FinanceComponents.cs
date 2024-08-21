using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Finances
{
    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct BasicMoneyAmount : IComponentData
    {
        public int Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct GhostFinancesPrefab : IComponentData
    {
        public Entity Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct FinancesEntity : IComponentData
    {
        public Entity Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct GhostFinancesConnectionId : IComponentData
    {
        public int Value;
    }

    public struct MoneyAmount : IComponentData
    {
        [GhostField] public int Value;
    }

}
