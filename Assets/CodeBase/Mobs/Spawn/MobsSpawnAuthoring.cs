using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.CodeBase.Mobs.Spawn
{
    [Serializable]
    public class TeamPoolSettings
    {
        public List<RoutePoolSettings> RouteSettings;
    }

    [Serializable]
    public class RoutePoolSettings
    {
        public List<Transform> Waypoints;
    }

    public class MobsSpawnAuthoring : MonoBehaviour
    {
        [SerializeField] private List<TeamPoolSettings> _teamPoolSettings;

        public List<TeamPoolSettings> TeamPoolSettings => _teamPoolSettings;

        public class MobsSpawnBaker : Baker<MobsSpawnAuthoring>
        {
            public override void Bake(MobsSpawnAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent<InitializeMobSpawnerTag>(entity);

                RoutePoolSettings mobWaypointSet = authoring.TeamPoolSettings[0].RouteSettings[0];

                using (BlobBuilder builder = new BlobBuilder(Allocator.Temp)) {
                    ref WaypointSet waypointSet = ref builder.ConstructRoot<WaypointSet>();

                    BlobBuilderArray<float3> arrayBuilder = builder.Allocate(
                        ref waypointSet.Waypoints,
                        mobWaypointSet.Waypoints.Count);

                    for (int k = 0; k < mobWaypointSet.Waypoints.Count; k++)
                        arrayBuilder[k] = mobWaypointSet.Waypoints[k].position;

                    BlobAssetReference<WaypointSet> blobReference =
                        builder.CreateBlobAssetReference<WaypointSet>(Allocator.Persistent);

                    AddBlobAsset(ref blobReference, out Unity.Entities.Hash128 hash);
                    AddComponent(entity, new WaypointSetReference { Blob = blobReference });
                }
            }
        }
    }

    public struct WaypointSet
    {
        public BlobArray<float3> Waypoints;
    }

    public struct WaypointSetReference : IComponentData
    {
        public BlobAssetReference<WaypointSet> Blob;
    }
}
