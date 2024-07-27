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

                AddComponent(entity, new WaypointSettingsReference {
                    Blob = CreateWaypointSettingsBlob(authoring.TeamPoolSettings)
                });
            }

            private BlobAssetReference<WaypointSettings> CreateWaypointSettingsBlob(List<TeamPoolSettings> teamPoolSettings) {
                BlobAssetReference<WaypointSettings> blobReference;

                using (BlobBuilder builder = new BlobBuilder(Allocator.Temp)) {
                    ref WaypointSettings waypointSettings = ref builder.ConstructRoot<WaypointSettings>();

                    waypointSettings.TeamAmount = (ushort)teamPoolSettings.Count;

                    BlobBuilderArray<ushort> routeAmountBuilder = builder.Allocate(
                        ref waypointSettings.RouteAmount,
                        teamPoolSettings.Count);

                    BlobBuilderArray<ushort> routeOffsetsBuilder = builder.Allocate(
                        ref waypointSettings.RouteOffsets,
                        teamPoolSettings.Count);

                    ushort
                        currentTeamRouteAmount,
                        routeCount = 0;
                    for (int i = 0; i < teamPoolSettings.Count; i++) {
                        routeOffsetsBuilder[i] = routeCount;

                        currentTeamRouteAmount = (ushort)teamPoolSettings[i].RouteSettings.Count;
                        routeCount += currentTeamRouteAmount;
                        routeAmountBuilder[i] = currentTeamRouteAmount;
                    }

                    BlobBuilderArray<ushort> waypointAmountBuilder = builder.Allocate(
                        ref waypointSettings.WaypointAmount,
                        routeCount);

                    BlobBuilderArray<ushort> waypointOffsetBuilder = builder.Allocate(
                        ref waypointSettings.WaypointOffsets,
                        routeCount);

                    ushort
                        currentBuilderIndex = 0,
                        currentRouteWaypointAmount,
                        waypointCount = 0;
                    for (int i = 0; i < teamPoolSettings.Count; i++) {
                        for (int j = 0; j < teamPoolSettings[i].RouteSettings.Count; j++, currentBuilderIndex++) {

                            waypointOffsetBuilder[currentBuilderIndex] = waypointCount;

                            currentRouteWaypointAmount = (ushort)teamPoolSettings[i].RouteSettings[j].Waypoints.Count;
                            waypointCount += currentRouteWaypointAmount;
                            waypointAmountBuilder[currentBuilderIndex] = currentRouteWaypointAmount;
                        }
                    }

                    BlobBuilderArray<float3> waypointBuilder = builder.Allocate(
                        ref waypointSettings.Waypoints,
                        waypointCount);

                    ushort currentWaypointIndex = 0;
                    for (int i = 0; i < teamPoolSettings.Count; i++)
                        for (int j = 0; j < teamPoolSettings[i].RouteSettings.Count; j++)
                            for (int k = 0; k < teamPoolSettings[i].RouteSettings[j].Waypoints.Count; k++, currentWaypointIndex++)
                                waypointBuilder[currentWaypointIndex] = teamPoolSettings[i].RouteSettings[j].Waypoints[k].position;

                    blobReference =
                        builder.CreateBlobAssetReference<WaypointSettings>(Allocator.Persistent);

                    AddBlobAsset(ref blobReference, out Unity.Entities.Hash128 hash);
                }

                return blobReference;
            }
        }
    }
}
