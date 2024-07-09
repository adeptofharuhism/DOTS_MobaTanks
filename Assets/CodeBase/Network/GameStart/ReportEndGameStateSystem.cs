using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Network.GameStart
{
    [UpdateInGroup(typeof(NetworkProcessSystemGroup))]
    [UpdateAfter(typeof(InGameStateSystem))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct ReportEndGameStateSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<ReportEndGame>();
            state.RequireForUpdate<WinnerTeam>();
        }

        public void OnUpdate(ref SystemState state) {
            Entity stateEntity = SystemAPI.GetSingletonEntity<ReportEndGame>();
            state.EntityManager.RemoveComponent<ReportEndGame>(stateEntity);

            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            Entity endGameRpc = ecb.CreateEntity();
            ecb.AddComponent(endGameRpc, new SendRpcCommandRequest());
            ecb.AddComponent(endGameRpc, new GoToEndGameStateRpc { Winner = Combat.Teams.TeamType.Blue });

            ecb.Playback(state.EntityManager);
        }
    }
}
