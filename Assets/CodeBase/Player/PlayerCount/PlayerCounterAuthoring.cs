using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Player.PlayerCount
{
    public class PlayerCounterAuthoring : MonoBehaviour
    {
        [SerializeField] private int _minReadyPlayersToStartGame = 1;

        public int MinReadyPlayersToStartGame => _minReadyPlayersToStartGame;

        public class PlayerCounterBaker : Baker<PlayerCounterAuthoring>
        {
            public override void Bake(PlayerCounterAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new MinReadyPlayersToStartGame { Value = authoring.MinReadyPlayersToStartGame });
                AddComponent(entity, new ConnectedPlayerCount { Value = 0 });
                AddComponent(entity, new ReadyPlayersCount { Value = 0 });
            }
        }
    }
}
