using Assets.CodeBase.Infrastructure.PrefabInjection;
using System;
using Unity.Entities;

namespace Assets.CodeBase.Network
{
    [UpdateInGroup(typeof(NetworkProcessSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial class DeployUiOnClientSystem : SystemBase
    {
        public Action OnReadyForUiDeploy;

        protected override void OnCreate() {
            RequireForUpdate<ReadyForUIDeployTag>();
        }

        protected override void OnUpdate() {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (tag, entity)
                in SystemAPI.Query<ReadyForUIDeployTag>()
                .WithEntityAccess()) {

                OnReadyForUiDeploy?.Invoke();

                ecb.RemoveComponent<ReadyForUIDeployTag>(entity);
            }

            ecb.Playback(EntityManager);
        }
    }
}
