using Assets.CodeBase.Combat.Teams;
using Assets.CodeBase.GameStates;
using Assets.CodeBase.Structures.Bases;
using Unity.Entities;

namespace Assets.CodeBase.Weapon.WeaponGroup
{
    [UpdateAfter(typeof(BaseSpawnSystem))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct InitializeWeaponGroupSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (weaponBuffer, container, team, entity)
                in SystemAPI.Query<DynamicBuffer<WeaponBufferElement>, WeaponContainer, UnitTeam>()
                .WithAll<ShouldInitializeWeaponGroup>()
                .WithEntityAccess()) {

                foreach (WeaponBufferElement weapon in weaponBuffer) {
                    Entity newWeapon = ecb.Instantiate(weapon.WeaponPrefab);

                    ecb.AddComponent(newWeapon, new WeaponSlot { Value = container.Value });
                    ecb.SetComponent(newWeapon, new UnitTeam { Value = team.Value });
                }

                ecb.RemoveComponent<ShouldInitializeWeaponGroup>(entity);
                ecb.RemoveComponent<WeaponContainer>(entity);
                weaponBuffer.Clear();
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
