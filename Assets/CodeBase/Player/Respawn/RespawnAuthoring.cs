using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Player.Respawn
{
    public class RespawnAuthoring : MonoBehaviour
    {
        [SerializeField] private float _respawnCooldown;

        public float RespawnCooldown => _respawnCooldown;

        public class RespawnBaker : Baker<RespawnAuthoring>
        {
            public override void Bake(RespawnAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent<VehicleRespawnParameters>(entity);
                
                AddComponent(entity, new RespawnCooldown { Value = authoring.RespawnCooldown });
                AddComponent<TimeToRespawn>(entity);

                AddComponent<ShouldRespawnTag>(entity);
            }
        }
    }
}
