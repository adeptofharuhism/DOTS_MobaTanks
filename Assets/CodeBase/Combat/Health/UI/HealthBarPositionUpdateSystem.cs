using Unity.Entities;
using Unity.Transforms;

namespace Assets.CodeBase.Combat.Health.UI
{
    [UpdateInGroup(typeof(HealthBarClientSystemGroup))]
    [UpdateAfter(typeof(HealthBarInitializationSystem))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct HealthBarPositionUpdateSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            foreach (var (transform, healthOffset, healthBarUI)
                in SystemAPI.Query<LocalTransform, HealthBarOffset, HealthBarUIReference>()) {

                healthBarUI.Value.transform.position = transform.Position + healthOffset.Value;
            }
        }
    }
}
