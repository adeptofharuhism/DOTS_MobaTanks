﻿using Assets.CodeBase.Utility;
using Unity.Entities;

namespace Assets.CodeBase.Finances
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(FinancesSystemGroup))]
    public partial class ClientMoneyUpdateSystem : SystemBase
    {
        public IReactiveGetter<int> Money => _money;

        private ReactiveProperty<int> _money = new(true);

        protected override void OnCreate() {
            RequireForUpdate<MoneyAmount>();
        }

        protected override void OnUpdate() {
            foreach (var moneyAmount
                in SystemAPI.Query<MoneyAmount>())
                _money.Value = moneyAmount.Value;
        }
    }
}
