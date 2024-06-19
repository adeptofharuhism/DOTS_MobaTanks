using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Infrastructure.PrefabInjection
{
    public struct SubScenesLoadPassedTag : IComponentData { }

    public struct GamePrefabs : IComponentData
    {
        public Entity Vehicle;
    }

    public class UIPrefabs : IComponentData
    {
        public GameObject HealthBar;
    }
}
