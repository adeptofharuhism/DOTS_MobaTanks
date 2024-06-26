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
                ecb.AddComponent(newWeapon, new WeaponParent { Value = entity });
                ecb.AddComponent<WeaponHasTurret>(newWeapon);

                ecb.AppendToBuffer(entity, new LinkedEntityGroup { Value = newWeapon });

                ecb.SetComponent(entity, new TurretWeapon { Value = newWeapon });
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
