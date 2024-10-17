using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Finances
{
    public class FinancesAuthoring : MonoBehaviour
    {
        [SerializeField] private int _basicMoneyAmount;
        [SerializeField] private int _moneyIncome;
        [SerializeField] private float _moneyIncomeCooldown;
        [SerializeField] private GameObject _ghostFinancesPrefab;

        public int BasicMoneyAmount => _basicMoneyAmount;
        public int MoneyIncome => _moneyIncome;
        public float MoneyIncomeCooldown => _moneyIncomeCooldown;
        public GameObject GhostFinancesPrefab => _ghostFinancesPrefab;

        public class FinancesBaker : Baker<FinancesAuthoring>
        {
            public override void Bake(FinancesAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new BasicMoneyAmount { Value = authoring.BasicMoneyAmount });

                AddComponent(entity, new MoneyIncome { Value = authoring.MoneyIncome });
                AddComponent(entity, new MoneyIncomeCooldown { Value = authoring.MoneyIncomeCooldown });
                AddComponent(entity, new MoneyIncomeTimeLeft { Value = authoring.MoneyIncomeCooldown });
                AddComponent<MoneyIncomeTag>(entity);
                SetComponentEnabled<MoneyIncomeTag>(entity, false);
            }
        }
    }
}
