using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Finances
{
    public class GhostFinancesAuthoring : MonoBehaviour
    {
        public class GhostFinancesBaker : Baker<GhostFinancesAuthoring>
        {
            public override void Bake(GhostFinancesAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent<MoneyAmount>(entity);
                AddBuffer<MoneyAddBufferElement>(entity);
                AddComponent<GhostFinancesConnectionId>(entity);
            }
        }
    }
}
