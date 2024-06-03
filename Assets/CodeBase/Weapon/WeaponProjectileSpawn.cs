using Assets.CodeBase.Targeting;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace Assets.CodeBase.Weapon
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct WeaponProjectileSpawnSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {

        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (projectilePrefab, currentTarget, projectileSpawnPoint)
                in SystemAPI.Query<WeaponProjectilePrefab, CurrentTarget, WeaponProjectileSpawnPoint>()) {

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
