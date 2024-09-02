using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Finances
{
    public class PlayerFinancesAuthoring : MonoBehaviour
    {
        public class GhostFinancesBaker : Baker<PlayerFinancesAuthoring>
        {
            public override void Bake(PlayerFinancesAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent<MoneyAmount>(entity);
                AddBuffer<MoneyAddBufferElement>(entity);
            }
        }
    }
}
