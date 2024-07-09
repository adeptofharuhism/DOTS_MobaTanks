using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.GameStates
{
    public class GameStateAuthoring : MonoBehaviour
    {
        public class GameStateBaker : Baker<GameStateAuthoring>
        {
            public override void Bake(GameStateAuthoring authoring) {
                Entity stateEntity = GetEntity(TransformUsageFlags.None);

                AddComponent<PrepareForGameState>(stateEntity);
            }
        }
    }
}
