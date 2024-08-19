using Assets.CodeBase.Combat.Teams;
using Assets.CodeBase.GameStates;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Assets.CodeBase.Weapon.WeaponGroup
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct InitializeWeaponGroupSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (weaponBuffer, container, team, entity)
                in SystemAPI.Query<DynamicBuffer<WeaponBufferElement>, WeaponGroupSlot, UnitTeam>()
                .WithAll<ShouldInitializeWeaponGroup>()
                .WithEntityAccess()) {

                foreach (WeaponBufferElement weapon in weaponBuffer) {
                    Entity newWeapon = ecb.Instantiate(weapon.WeaponPrefab);

                    ecb.SetComponent(newWeapon, LocalTransform.FromPosition(float3.zero));
                    ecb.AddComponent(newWeapon, new Parent { Value = container.Value });
                    ecb.SetComponent(newWeapon, new UnitTeam { Value = team.Value });

                    ecb.AppendToBuffer(entity, new LinkedEntityGroup { Value = newWeapon });
                }

                ecb.RemoveComponent<ShouldInitializeWeaponGroup>(entity);
                ecb.RemoveComponent<WeaponGroupSlot>(entity);
                weaponBuffer.Clear();
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
