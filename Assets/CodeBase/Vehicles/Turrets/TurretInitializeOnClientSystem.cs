using Assets.CodeBase.Combat.Teams;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Assets.CodeBase.Vehicles.Turrets
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct TurretInitializeOnClientSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (modelPrefab, slot, team, entity)
                in SystemAPI.Query<TurretModelPrefab, TurretSlot, UnitTeam>()
                .WithAll<TurretUninitialized>()
                .WithEntityAccess()) {

                ecb.RemoveComponent<TurretUninitialized>(entity);
                ecb.AddComponent<TurretModelInitialized>(entity);

                Entity newModel = ecb.Instantiate(modelPrefab.Value);

                ecb.SetComponent(newModel, new UnitTeam { Value = team.Value });

                ecb.SetComponent(newModel, LocalTransform.FromPosition(float3.zero));
                ecb.AddComponent(newModel, new Parent { Value = slot.Value });

                ecb.AppendToBuffer(entity, new LinkedEntityGroup { Value = newModel });

                ecb.SetComponent(entity, new TurretModel { Value = newModel });
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
