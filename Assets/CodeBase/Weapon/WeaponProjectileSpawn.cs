﻿using Assets.CodeBase.Targeting;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace Assets.CodeBase.Weapon
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct WeaponCooldownSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (cooldown, timeOnCooldown, weapon)
                in SystemAPI.Query<WeaponCooldown, RefRW<WeaponTimeOnCooldown>>()
                .WithAll<WeaponOnCooldownTag>()
                .WithEntityAccess()) {

                timeOnCooldown.ValueRW.Value += SystemAPI.Time.DeltaTime;

                if (timeOnCooldown.ValueRW.Value > cooldown.Value) {
                    timeOnCooldown.ValueRW.Value = 0;

                    ecb.AddComponent<WeaponReadyToFireTag>(weapon);
                    ecb.RemoveComponent<WeaponOnCooldownTag>(weapon);
                }
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    [UpdateAfter(typeof(WeaponCooldownSystem))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct WeaponProjectileSpawnSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (projectilePrefab, currentTarget, projectileSpawnPoint, weapon)
                in SystemAPI.Query<WeaponProjectilePrefab, CurrentTarget, WeaponProjectileSpawnPoint>()
                .WithAll<WeaponReadyToFireTag>()
                .WithEntityAccess()) {

                ecb.AddComponent<WeaponOnCooldownTag>(weapon);
                ecb.RemoveComponent<WeaponReadyToFireTag>(weapon);

                if (currentTarget.Value == Entity.Null)
                    continue;

                RefRO<LocalToWorld> spawnPointTransform = SystemAPI.GetComponentRO<LocalToWorld>(projectileSpawnPoint.Value);
                RefRO<LocalToWorld> currentTargetTransform = SystemAPI.GetComponentRO<LocalToWorld>(currentTarget.Value);

                LocalTransform projectileSpawnTransform =
                    LocalTransform.FromPositionRotation(
                        spawnPointTransform.ValueRO.Position,
                        quaternion.LookRotationSafe(
                            math.normalize(currentTargetTransform.ValueRO.Position - spawnPointTransform.ValueRO.Position),
                            math.up()));

                Entity projectile = ecb.Instantiate(projectilePrefab.Value);

                ecb.SetComponent(projectile, projectileSpawnTransform);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}