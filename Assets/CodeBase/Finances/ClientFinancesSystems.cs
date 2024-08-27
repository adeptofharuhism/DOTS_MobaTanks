using System;
using Unity.Entities;

namespace Assets.CodeBase.Finances
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(FinancesSystemGroup))]
    public partial class ClientMoneyUpdateSystem : SystemBase
    {
        public Action<int> OnMoneyValueChanged;

        private int _moneyAmount;
        private int MoneyAmount {
            set {
                if (value != _moneyAmount)
                    OnMoneyValueChanged?.Invoke(value);
                _moneyAmount = value;
            }
        }

        protected override void OnCreate() {
            RequireForUpdate<MoneyAmount>();
        }

        protected override void OnUpdate() {
            foreach (var moneyAmount
                in SystemAPI.Query<MoneyAmount>())
                MoneyAmount = moneyAmount.Value;
        }
    }
}
