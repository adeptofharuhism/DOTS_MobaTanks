﻿using Assets.CodeBase.Combat.Health;
using Assets.CodeBase.Infrastructure.Destruction;
using Assets.CodeBase.GameStates;
using Unity.Burst;
using Unity.Entities;

namespace Assets.CodeBase.Combat.Damage
{
    
    [UpdateInGroup(typeof(CombatSimulationSystemGroup), OrderLast = true)]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct DamageApplySystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (frameDamage, health, entity)
                in SystemAPI.Query<RefRW<DamageThisFrame>, RefRW<CurrentHealthPoints>>()
                .WithEntityAccess()) {

                health.ValueRW.Value -= frameDamage.ValueRW.Value;
                frameDamage.ValueRW.Value = 0;

                if (health.ValueRW.Value <= 0)
                    ecb.AddComponent<DestroyEntityTag>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
