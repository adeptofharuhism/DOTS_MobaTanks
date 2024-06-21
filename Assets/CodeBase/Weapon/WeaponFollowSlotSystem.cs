using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace Assets.CodeBase.Weapon
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct WeaponFollowSlotSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            foreach (var (transform, slot)
                in SystemAPI.Query<RefRW<LocalTransform>, WeaponSlot>()) {

                RefRO<LocalToWorld> slotTransform = SystemAPI.GetComponentRO<LocalToWorld>(slot.Value);

                transform.ValueRW.Position = slotTransform.ValueRO.Position;
            }
        }
    }
}
