using Assets.CodeBase.GameStates;
using Assets.CodeBase.Targeting;
using Assets.CodeBase.Teams;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Assets.CodeBase.Weapon
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(TargetingActivationSystemGroup))]
    public partial struct WeaponCooldownSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (cooldown, timeOnCooldown, weapon)
                in SystemAPI.Query<WeaponCooldown, RefRW<WeaponTimeOnCooldown>>()
                .WithEntityAccess()) {

                timeOnCooldown.ValueRW.Value -= SystemAPI.Time.DeltaTime;

                if (timeOnCooldown.ValueRW.Value > 0)
                    continue;

                timeOnCooldown.ValueRW.Value = cooldown.Value;

                ecb.SetComponentEnabled<Targeter>(weapon, true);
                ecb.SetComponentEnabled<WeaponReadyToFireTag>(weapon, true);
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(WeaponSystemGroup))]
    [UpdateBefore(typeof(WeaponProjectileSpawnSystem))]
    public partial struct InitializeWeaponGroupSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (weaponBuffer, container, team, entity)
                in SystemAPI.Query<DynamicBuffer<WeaponBufferElement>, WeaponGroupSlot, UnitTeam>()
                .WithAll<ShouldInitializeWeaponGroup>()
                .WithEntityAccess()) {

                foreach (WeaponBufferElement weapon in weaponBuffer) {
                    Entity newWeapon = ecb.Instantiate(weapon.WeaponPrefab);

                    ecb.SetComponent(newWeapon, LocalTransform.FromPosition(float3.zero));
                    ecb.AddComponent(newWeapon, new Parent { Value = container.Value });
                    ecb.SetComponent(newWeapon, new UnitTeam { Value = team.Value });

                    ecb.AppendToBuffer(entity, new LinkedEntityGroup { Value = newWeapon });
                }

                ecb.RemoveComponent<ShouldInitializeWeaponGroup>(entity);
                ecb.RemoveComponent<WeaponGroupSlot>(entity);
                weaponBuffer.Clear();
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(WeaponSystemGroup))]
    [UpdateAfter(typeof(InitializeWeaponGroupSystem))]
    public partial struct WeaponProjectileSpawnSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (projectilePrefab, currentTarget, projectileSpawnPoint, team, weapon)
                in SystemAPI.Query<WeaponProjectilePrefab, CurrentTarget, WeaponProjectileSpawnPoint, UnitTeam>()
                .WithAll<WeaponReadyToFireTag>()
                .WithEntityAccess()) {

                ecb.SetComponentEnabled<WeaponReadyToFireTag>(weapon, false);

                if (!state.EntityManager.Exists(currentTarget.Value))
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
                ecb.SetComponent(projectile, new UnitTeam { Value = team.Value });
            }

            ecb.Playback(state.EntityManager);
        }
    }
}