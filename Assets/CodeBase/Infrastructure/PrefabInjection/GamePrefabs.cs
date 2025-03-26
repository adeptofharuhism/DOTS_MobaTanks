using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Infrastructure.PrefabInjection
{
    public struct SubScenesLoadPassedTag : IComponentData { }
    public struct ReadyForUIDeployTag : IComponentData { }

    public struct GamePrefabs : IComponentData
    {
        public Entity Player;
        public Entity Vehicle;
        public Entity Base;
        
        public Entity HealthBar;
        public Entity VehicleHealthBar;
    }
}
