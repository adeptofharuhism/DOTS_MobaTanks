using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Infrastructure.PrefabInjection
{
    public class GamePrefabsAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject Vehicle;

        public class GamePrefabsBaker : Baker<GamePrefabsAuthoring>
        {
            public override void Bake(GamePrefabsAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new GamePrefabs {
                    Vehicle = GetEntity(authoring.Vehicle, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}