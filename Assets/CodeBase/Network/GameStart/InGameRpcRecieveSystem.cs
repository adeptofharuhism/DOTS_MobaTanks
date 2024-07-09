using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Network.GameStart
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct InGameRpcRecieveSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            EntityQueryBuilder inGameCommandQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<GoToInGameStateRpc, ReceiveRpcCommandRequest>();
            state.RequireForUpdate(state.GetEntityQuery(inGameCommandQuery));
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (commandSource, commandEntity)
                in SystemAPI.Query<ReceiveRpcCommandRequest>()
                .WithAll<GoToInGameStateRpc>()
                .WithEntityAccess()) {

                ecb.DestroyEntity(commandEntity);

                ecb.AddComponent<InGame>(ecb.CreateEntity());
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
