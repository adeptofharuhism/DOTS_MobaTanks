using Assets.CodeBase.GameStates;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Finances
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(FinancesSystemGroup))]
    public partial struct GhostFinancesRelevancySetSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
            state.RequireForUpdate<GhostRelevancy>();
        }

        public void OnUpdate(ref SystemState state) {
            NativeParallelHashMap<RelevantGhostForConnection, int> ghostRelevancySet = 
                SystemAPI.GetSingleton<GhostRelevancy>().GhostRelevancySet;

            
        }
    }
}
