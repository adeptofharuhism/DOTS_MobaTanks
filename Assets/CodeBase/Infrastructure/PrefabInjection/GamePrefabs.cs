using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Infrastructure.PrefabInjection
{
    public struct SubScenesLoadPassedTag : IComponentData { }
    public struct ReadyForUIDeployTag : IComponentData { }

    public struct GamePrefabs : IComponentData
    {
        public Entity Vehicle;
        public Entity Base;
    }

    public class UIPrefabs : IComponentData
    {
        public GameObject HealthBar;
        public GameObject VehicleHealthBar;
    }
}
