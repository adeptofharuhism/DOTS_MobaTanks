using Unity.Entities;
using UnityEngine;

namespace Assets.CodeBase.Vehicles.Wheels
{
    public class WheelModelUpdateCountdownAuthoring : MonoBehaviour
    {
        [SerializeField] private int _updatesPerSecond = 60;

        public int UpdatesPerSecond => _updatesPerSecond;

        private class Baker : Baker<WheelModelUpdateCountdownAuthoring>
        {
            public override void Bake(WheelModelUpdateCountdownAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new WheelModelUpdateCountdown {
                    AwaitTime = 1f / authoring.UpdatesPerSecond,
                    TimeLeftForNextUpdate = 0
                });
            }
        }
    }
}