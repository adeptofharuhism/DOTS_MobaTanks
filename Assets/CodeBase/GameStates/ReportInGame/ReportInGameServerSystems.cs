using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.GameStates.ReportInGame
{
    [UpdateInGroup(typeof(ReportInGameStateSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct SendRpcGameStartedSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<ReportInGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            state.EntityManager.CreateEntity(typeof(GoToInGameStateRpc), typeof(SendRpcCommandRequest));
        }
    }

    [UpdateInGroup(typeof(ReportInGameStateSystemGroup), OrderLast = true)]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct EnterInGameStateSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<ReportInGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            Entity stateEntity = SystemAPI.GetSingletonEntity<ReportInGameState>();
            state.EntityManager.RemoveComponent<ReportInGameState>(stateEntity);
            state.EntityManager.AddComponent<InGameState>(stateEntity);
        }
    }
}
