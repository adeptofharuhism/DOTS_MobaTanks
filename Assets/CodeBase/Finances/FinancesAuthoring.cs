using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Finances
{
    public class FinancesAuthoring : MonoBehaviour
    {
        [SerializeField] private int _basicMoneyAmount;
        [SerializeField] private GameObject _ghostFinancesPrefab;

        public int BasicMoneyAmount => _basicMoneyAmount;
        public GameObject GhostFinancesPrefab => _ghostFinancesPrefab;

        public class FinancesBaker : Baker<FinancesAuthoring>
        {
            public override void Bake(FinancesAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new BasicMoneyAmount { Value = authoring.BasicMoneyAmount });
                AddComponent(entity, new GhostFinancesPrefab {
                    Value = GetEntity(authoring.GhostFinancesPrefab, TransformUsageFlags.None)
                });
            }
        }
    }
}
