﻿using Assets.CodeBase.GameStates;
using Unity.Burst;
using Unity.Entities;

namespace Assets.CodeBase.Mobs.Spawn.Spawner
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(SpawnSystemGroup))]
    [UpdateBefore(typeof(SelectMobSpawnPositionSystem))]
    public partial struct UpdateMobSpawnCooldownSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (cooldown, cooldownTimeLeft, entity)
                in SystemAPI.Query<MobSpawnCooldown, RefRW<MobSpawnCooldownTimeLeft>>()
                .WithEntityAccess()) {

                cooldownTimeLeft.ValueRW.Value -= SystemAPI.Time.DeltaTime;

                if (cooldownTimeLeft.ValueRO.Value > 0)
                    continue;

                cooldownTimeLeft.ValueRW.Value = cooldown.Value;

                ecb.AddComponent<ShouldSpawnMobTag>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
