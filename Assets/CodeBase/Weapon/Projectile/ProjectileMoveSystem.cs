using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

namespace Assets.CodeBase.Weapon.Projectile
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
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