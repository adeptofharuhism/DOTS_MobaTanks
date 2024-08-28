using Assets.CodeBase.Finances;
using Unity.Entities;

namespace Assets.CodeBase.Shop
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(FinancesSystemGroup))]
    public partial class ShopSystemGroup : ComponentSystemGroup { }
}
