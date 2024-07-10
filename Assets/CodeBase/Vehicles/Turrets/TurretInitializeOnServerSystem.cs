using Assets.CodeBase.Combat.Teams;
using Assets.CodeBase.Weapon;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Assets.CodeBase.Vehicles.Turrets
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct TurretInitializeOnServerSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (weaponPrefab, slot, team, entity)
                in SystemAPI.Query<TurretWeaponPrefab, TurretSlot, UnitTeam>()
                .WithAll<TurretUninitialized>()
                .WithEntityAccess()) {

                ecb.RemoveComponent<TurretUninitialized>(entity);

                Entity newWeapon = ecb.Instantiate(weaponPrefab.Value);

                ecb.SetComponent(newWeapon, LocalTransform.FromPosition(float3.zero));

                ecb.SetComponent(newWeapon, new UnitTeam { Value = team.Value });

                ecb.AddComponent<WeaponHasTurret>(newWeapon);
                ecb.AddComponent(newWeapon, new Parent { Value = slot.Value });
                ecb.AddComponent(newWeapon, new WeaponsVehicleParentEntity { Value = entity });

                ecb.AppendToBuffer(entity, new LinkedEntityGroup { Value = newWeapon });

                ecb.SetComponent(entity, new TurretWeapon { Value = newWeapon });
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
