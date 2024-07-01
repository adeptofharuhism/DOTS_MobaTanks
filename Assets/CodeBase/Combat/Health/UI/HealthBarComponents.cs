using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.CodeBase.Combat.Health.UI
{
    [GhostComponent(PrefabType = GhostPrefabType.Client)]
    public class HealthBarUIReference : ICleanupComponentData
    {
        public GameObject Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Client)]
    public class HealthBarSliderReference : ICleanupComponentData
    {
        public Slider Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Client)]
    public class HealthBarColorReference : ICleanupComponentData
    {
        public HealthBarColor Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Client)]
    public class HealthBarPlayerNameReference : ICleanupComponentData
    {
        public HealthBarPlayerName Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Client)]
    public class HealthBarCounterReference : ICleanupComponentData
    {
        public HealthBarCounter Value;
    }
}
