using Unity.Entities;

namespace Assets.CodeBase.Infrastructure.PrefabInjection
{
    public struct TimeForPrefabsToPreparePassedTag : IComponentData { }

    public struct GamePrefabs : IComponentData
    {
        public Entity Vehicle;
    }
}
