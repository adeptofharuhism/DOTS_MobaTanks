using Assets.CodeBase.Combat.Teams;
using Assets.CodeBase.GameStates;
using Assets.CodeBase.GameStates.InGame;
using Unity.Burst;
using Unity.Entities;

namespace Assets.CodeBase.Structures.Bases
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct BaseCleanUpSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (loserTeam, entity)
                in SystemAPI.Query<BaseTeamCleanUp>()
                .WithNone<BaseTag>()
                .WithEntityAccess()) {

                Entity winningTeamEntity = ecb.CreateEntity();
                ecb.AddComponent(winningTeamEntity, new WinnerTeam {
                    Value = loserTeam.Team == TeamType.Blue ? TeamType.Orange : TeamType.Blue
                });

                ecb.RemoveComponent<BaseTeamCleanUp>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
