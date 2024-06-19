using Unity.Entities;

namespace Assets.CodeBase.Combat.Health
{
    [UpdateInGroup(typeof(HealthBarClientSystemGroup))]
    [UpdateAfter(typeof(HealthBarInitializationSystem))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct HealthBarValueUpdateSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            foreach (var (currentHealth, maxHealth, healthSlider)
                in SystemAPI.Query<CurrentHealthPoints, MaximalHealthPoints, HealthBarSliderReference>()) {

                healthSlider.Value.minValue = 0;
                healthSlider.Value.maxValue = maxHealth.Value;
                healthSlider.Value.value = currentHealth.Value;
            }
        }
    }
}
