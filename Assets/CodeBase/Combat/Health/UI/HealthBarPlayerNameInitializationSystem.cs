using Unity.Entities;

namespace Assets.CodeBase.Combat.Health.UI
{
    [UpdateInGroup(typeof(HealthBarClientSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct HealthBarPlayerNameInitializationSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (playerName, uiPlayerName, entity)
                in SystemAPI.Query<PlayerName, HealthBarPlayerNameReference>()
                .WithAll<InitializePlayerNameTag>()
                .WithEntityAccess()) {

                uiPlayerName.Value.SetPlayerName(playerName.Value.ToString());

                ecb.RemoveComponent<InitializePlayerNameTag>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
