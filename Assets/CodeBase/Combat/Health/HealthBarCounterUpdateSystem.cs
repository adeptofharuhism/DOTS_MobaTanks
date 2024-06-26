using Unity.Entities;

namespace Assets.CodeBase.Combat.Health
{
    [UpdateInGroup(typeof(HealthBarClientSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct HealthBarCounterUpdateSystem : ISystem {
        public void OnUpdate(ref SystemState state) {
            foreach (var (currentHealth, uiCounter)
                in SystemAPI.Query<CurrentHealthPoints, HealthBarCounterReference>()) {

                uiCounter.Value.SetHealthCount(currentHealth.Value);
            }
        }
    }
}
