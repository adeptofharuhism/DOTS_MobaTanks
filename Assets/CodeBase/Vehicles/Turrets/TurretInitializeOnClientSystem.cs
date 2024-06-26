using Assets.CodeBase.Combat.Teams;
using Unity.Entities;
using Unity.Transforms;

namespace Assets.CodeBase.Vehicles.Turrets
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct TurretInitializeOnClientSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (modelPrefab, slot, team, entity)
                in SystemAPI.Query<TurretModelPrefab, TurretSlot, UnitTeam>()
                .WithAll<TurretUninitialized>()
                .WithEntityAccess()) {

                ecb.RemoveComponent<TurretUninitialized>(entity);
                ecb.AddComponent<TurretModelInitialized>(entity);

                Entity newModel = ecb.Instantiate(modelPrefab.Value);

                RefRO<LocalToWorld> slotTransform = SystemAPI.GetComponentRO<LocalToWorld>(slot.Value);
                LocalTransform modelTransform = LocalTransform.FromPosition(slotTransform.ValueRO.Position);
                ecb.SetComponent(newModel, modelTransform);

                ecb.SetComponent(newModel, new UnitTeam { Value = team.Value });

                ecb.AppendToBuffer(entity, new LinkedEntityGroup { Value = newModel });

                ecb.SetComponent(entity, new TurretModel { Value = newModel });
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
