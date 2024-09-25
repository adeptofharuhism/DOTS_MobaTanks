using Assets.CodeBase.GameStates;
using Assets.CodeBase.Infrastructure.PrefabInjection;
using System;
using Unity.Entities;

namespace Assets.CodeBase.UI
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(UiSystemGroup))]
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

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(UiSystemGroup))]
    public partial class InGameUiActivationSystem : SystemBase
    {
        public Action OnGameStart;

        protected override void OnCreate() {
            RequireForUpdate<InGameState>();
        }

        protected override void OnUpdate() {
            OnGameStart?.Invoke();

            Enabled = false;
        }
    }
}
