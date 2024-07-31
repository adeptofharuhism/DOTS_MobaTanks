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

    [Serializable]
    public class InitialMobWaveSettings
    {
        public GameObject MobPrefab;
        public int Amount;
        public int WaveCooldown;
    }

    public class MobsSpawnAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject _mobSpawnerPrefab;
        [SerializeField] private List<InitialMobWaveSettings> _initialMobWaves;
        [SerializeField] private List<TeamPoolSettings> _teamPoolSettings;

        public GameObject MobSpawnerPrefab => _mobSpawnerPrefab;
        public List<InitialMobWaveSettings> InitialMobWaves => _initialMobWaves;
        public List<TeamPoolSettings> TeamPoolSettings => _teamPoolSettings;

        public class MobsSpawnBaker : Baker<MobsSpawnAuthoring>
        {
            public override void Bake(MobsSpawnAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent<InitializeSpawnRequestProcessTag>(entity);

                AddBuffer<NewSpawnerInstantiationParametersElement>(entity);
                AddComponent(entity, new SpawnerPrefab {
                    Value = GetEntity(authoring.MobSpawnerPrefab, TransformUsageFlags.None)
                });
                CreateInitialMobSpawnRequests(entity, authoring.TeamPoolSettings, authoring.InitialMobWaves);

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

            private void CreateInitialMobSpawnRequests(
                Entity entity,
                List<TeamPoolSettings> teamPoolSettings,
                List<InitialMobWaveSettings> initialMobWaves) {

                DynamicBuffer<NewSpawnRequestElement> mobSpawnBuffer = AddBuffer<NewSpawnRequestElement>(entity);

                for (ushort i = 0; i < teamPoolSettings.Count; i++)
                    foreach (InitialMobWaveSettings mobWave in initialMobWaves)
                        for (int j = 0; j < mobWave.Amount; j++)
                            mobSpawnBuffer.Add(new NewSpawnRequestElement {
                                MobPrefab = GetEntity(mobWave.MobPrefab, TransformUsageFlags.Dynamic),
                                WaveCooldown = (ushort)mobWave.WaveCooldown,
                                Team = i
                            });
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
