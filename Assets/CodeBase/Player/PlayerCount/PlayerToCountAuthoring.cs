using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Player.PlayerCount
{
    public class PlayerToCountAuthoring : MonoBehaviour
    {
        public class PlayerToCountBaker : Baker<PlayerToCountAuthoring>
        {
            public override void Bake(PlayerToCountAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent<CountAsPlayerTag>(entity);
                AddComponent<DecreaseConnectedPlayerCountOnCleanUpTag>(entity);
            }
        }
    }
}
