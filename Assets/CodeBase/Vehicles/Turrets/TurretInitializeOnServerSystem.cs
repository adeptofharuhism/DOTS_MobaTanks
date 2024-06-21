using Assets.CodeBase.Combat.Teams;
using Assets.CodeBase.Weapon;
using Unity.Entities;
using Unity.Transforms;

namespace Assets.CodeBase.Vehicles.Turrets
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct TurretInitializeOnServerSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (weaponPrefab, slot, team, entity)
                in SystemAPI.Query<TurretWeaponPrefab, TurretSlot, UnitTeam>()
                .WithAll<TurretUninitialized>()
                .WithEntityAccess()) {

                ecb.RemoveComponent<TurretUninitialized>(entity);

                Entity newWeapon = ecb.Instantiate(weaponPrefab.Value);

                RefRO<LocalTransform> slotTransform = SystemAPI.GetComponentRO<LocalTransform>(slot.Value);
                LocalTransform weaponTransform = LocalTransform.FromPosition(slotTransform.ValueRO.Position);
                ecb.SetComponent(newWeapon, weaponTransform);

                ecb.SetComponent(newWeapon, new UnitTeam { Value = team.Value });

                ecb.AddComponent(newWeapon, new WeaponSlot { Value = slot.Value });

                ecb.SetComponent(entity, new TurretWeapon { Value = newWeapon });
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct TurretInitializeOnClientSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (model, modelPrefab, slot, entity)
                in SystemAPI.Query<RefRW<TurretModel>, TurretModelPrefab, TurretSlot>()
                .WithAll<TurretUninitialized>()
                .WithEntityAccess()) {

                ecb.RemoveComponent<TurretUninitialized>(entity);
                ecb.AddComponent<TurretModelInitialized>(entity);

                Entity newModel = ecb.Instantiate(modelPrefab.Value);

                RefRO<LocalToWorld> slotTransform = SystemAPI.GetComponentRO<LocalToWorld>(slot.Value);
                LocalTransform modelTransform = LocalTransform.FromPosition(slotTransform.ValueRO.Position);
                ecb.SetComponent(newModel, modelTransform);

                ecb.SetComponent(entity, new TurretModel { Value = newModel });
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct TurretFollowSlotSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            foreach (var (model, slot)
                in SystemAPI.Query<TurretModel, TurretSlot>()
                .WithAll<TurretModelInitialized>()) {

                RefRO<LocalToWorld> slotTransform = SystemAPI.GetComponentRO<LocalToWorld>(slot.Value);
                RefRW<LocalTransform> modelTransform = SystemAPI.GetComponentRW<LocalTransform>(model.Value);

                modelTransform.ValueRW.Position = slotTransform.ValueRO.Position;
                modelTransform.ValueRW.Rotation = slotTransform.ValueRO.Rotation;
            }
        }
    }
}
