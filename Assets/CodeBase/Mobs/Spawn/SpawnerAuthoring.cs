using Assets.CodeBase.Combat.Teams;
using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Mobs.Spawn
{
    public class SpawnerAuthoring : MonoBehaviour
    {
        public class SpawnerBaker : Baker<SpawnerAuthoring>
        {
            public override void Bake(SpawnerAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent<MobPrefab>(entity);

                AddComponent<CurrentRoute>(entity);
                AddComponent<RouteAmount>(entity);
                AddComponent<RouteOffset>(entity);

                AddComponent<MobSpawnPosition>(entity);

                AddComponent<MobSpawnCooldown>(entity);
                AddComponent<MobSpawnCooldownTimeLeft>(entity);

                AddComponent<UnitTeam>(entity);
                AddComponent<WaypointSettingsReference>(entity);
            }
        }
    }
}
