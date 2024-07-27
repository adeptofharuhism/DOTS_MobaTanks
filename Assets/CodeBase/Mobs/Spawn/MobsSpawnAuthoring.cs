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

                BlobBuilder builder = new BlobBuilder(Allocator.Temp);

                ref WaypointSettings waypointSettings = ref builder.ConstructRoot<WaypointSettings>();

                waypointSettings.TeamAmount = (ushort)teamPoolSettings.Count;

                SetWaypoints(ref builder, ref waypointSettings, teamPoolSettings);

                blobReference =
                    builder.CreateBlobAssetReference<WaypointSettings>(Allocator.Persistent);

                AddBlobAsset(ref blobReference, out Unity.Entities.Hash128 hash);

                builder.Dispose();

                return blobReference;
            }

            private void SetWaypoints(
                ref BlobBuilder builder,
                ref WaypointSettings waypointSettings,
                List<TeamPoolSettings> teamPoolSettings) {

                ushort waypointCount = SetWaypointOffsets(ref builder, ref waypointSettings, teamPoolSettings);

                BlobBuilderArray<float3> waypointBuilder = builder.Allocate(
                    ref waypointSettings.Waypoints,
                    waypointCount);

                ushort currentWaypointIndex = 0;
                for (int i = 0; i < teamPoolSettings.Count; i++)
                    for (int j = 0; j < teamPoolSettings[i].RouteSettings.Count; j++)
                        for (int k = 0; k < teamPoolSettings[i].RouteSettings[j].Waypoints.Count; k++, currentWaypointIndex++)
                            waypointBuilder[currentWaypointIndex] = teamPoolSettings[i].RouteSettings[j].Waypoints[k].position;
            }

            private ushort SetWaypointOffsets(
                ref BlobBuilder builder,
                ref WaypointSettings waypointSettings,
                List<TeamPoolSettings> teamPoolSettings) {

                ushort routeCount = SetRouteOffsets(ref builder, ref waypointSettings, teamPoolSettings);

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

                return waypointCount;
            }

            private ushort SetRouteOffsets(
                ref BlobBuilder builder,
                ref WaypointSettings waypointSettings,
                List<TeamPoolSettings> teamPoolSettings) {

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

                return routeCount;
            }
        }
    }
}
