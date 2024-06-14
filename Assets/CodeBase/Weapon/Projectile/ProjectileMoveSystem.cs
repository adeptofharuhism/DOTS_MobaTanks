using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace Assets.CodeBase.Weapon.Projectile
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(ProjectileCollisionDetectionSystem))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct ProjectileMoveSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            foreach (var (transform, speed)
                in SystemAPI.Query<RefRW<LocalTransform>, ProjectileSpeed>()) {

                transform.ValueRW.Position += transform.ValueRW.Forward() * speed.Value * SystemAPI.Time.DeltaTime;
            }
        }
    }
}