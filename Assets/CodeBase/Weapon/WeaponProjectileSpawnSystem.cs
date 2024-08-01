using Assets.CodeBase.Combat.Teams;
using Assets.CodeBase.GameStates;
using Assets.CodeBase.Targeting;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace Assets.CodeBase.Weapon
{

    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    [UpdateAfter(typeof(WeaponCooldownSystem))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct WeaponProjectileSpawnSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (projectilePrefab, currentTarget, projectileSpawnPoint, team, weapon)
                in SystemAPI.Query<WeaponProjectilePrefab, CurrentTarget, WeaponProjectileSpawnPoint, UnitTeam>()
                .WithAll<WeaponReadyToFireTag>()
                .WithEntityAccess()) {

                ecb.RemoveComponent<WeaponReadyToFireTag>(weapon);
                ecb.RemoveComponent<Targeter>(weapon);

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
