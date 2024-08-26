using Assets.CodeBase.Finances;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Relevancy
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(RelevancySystemGroup))]
    public partial struct ClearRelevancySetSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<GhostRelevancy>();
        }

        public void OnUpdate(ref SystemState state) {
            NativeParallelHashMap<RelevantGhostForConnection, int> relevancySet =
                SystemAPI.GetSingleton<GhostRelevancy>().GhostRelevancySet;

            relevancySet.Clear();
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(RelevancySystemGroup))]
    [UpdateAfter(typeof(ClearRelevancySetSystem))]
    public partial struct FinancesRelevancySystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<GhostRelevancy>();
        }

        public void OnUpdate(ref SystemState state) {
            NativeParallelHashMap<RelevantGhostForConnection, int> relevancySet = 
                SystemAPI.GetSingleton<GhostRelevancy>().GhostRelevancySet;

            foreach (var (connectionId, ghostInstance)
                in SystemAPI.Query<GhostFinancesConnectionId, GhostInstance>()) {

                if (ghostInstance.ghostId == 0)
                    continue;

                foreach (var networkId
                    in SystemAPI.Query<NetworkId>()) {

                    if (networkId.Value == connectionId.Value)
                        continue;

                    relevancySet.Add(
                        new RelevantGhostForConnection { Connection = networkId.Value, Ghost = ghostInstance.ghostId },
                        connectionId.Value);
                }
            }
        }
    }
}
