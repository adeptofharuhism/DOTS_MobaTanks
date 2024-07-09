using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Network.GameStart
{
    [UpdateInGroup(typeof(NetworkProcessSystemGroup))]
    [UpdateAfter(typeof(CountingPlayersStateSystem))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct ReportInGameStateSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<ReportInGame>();
        }

        public void OnUpdate(ref SystemState state) {
            Entity stateEntity = SystemAPI.GetSingletonEntity<ReportInGame>();

            state.EntityManager.CreateEntity(typeof(GoToInGameStateRpc), typeof(SendRpcCommandRequest));

            state.EntityManager.RemoveComponent<ReportInGame>(stateEntity);
            state.EntityManager.AddComponent<InGame>(stateEntity);
        }
    }
}
