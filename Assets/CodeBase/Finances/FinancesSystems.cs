using Assets.CodeBase.GameStates;
using Unity.Entities;

namespace Assets.CodeBase.Finances
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(FinancesSystemGroup))]
    [UpdateBefore(typeof(MoneyIncomeApplySystem))]
    public partial struct MoneyIncomeCooldownSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new(Unity.Collections.Allocator.Temp);

            foreach (var (timeLeft, cooldown, entity)
                in SystemAPI.Query<RefRW<MoneyIncomeTimeLeft>, MoneyIncomeCooldown>()
                .WithEntityAccess()) {

                timeLeft.ValueRW.Value -= SystemAPI.Time.DeltaTime;

                if (timeLeft.ValueRO.Value > 0)
                    continue;

                timeLeft.ValueRW.Value = cooldown.Value;

                ecb.SetComponentEnabled<MoneyIncomeTag>(entity, true);
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(FinancesSystemGroup))]
    [UpdateAfter(typeof(MoneyIncomeCooldownSystem))]
    [UpdateBefore(typeof(AddMoneySystem))]
    public partial struct MoneyIncomeApplySystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer ecb = new(Unity.Collections.Allocator.Temp);

            foreach (var (income, entity)
                in SystemAPI.Query<MoneyIncome>()
                .WithAll<MoneyIncomeTag>()
                .WithEntityAccess()) {

                foreach (var moneyBuffer
                    in SystemAPI.Query<DynamicBuffer<MoneyAddBufferElement>>())
                    moneyBuffer.Add(new MoneyAddBufferElement { Value = income.Value });

                ecb.SetComponentEnabled<MoneyIncomeTag>(entity, false);
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(FinancesSystemGroup))]
    [UpdateAfter(typeof(MoneyIncomeApplySystem))]
    public partial struct AddMoneySystem : ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InGameState>();
        }

        public void OnUpdate(ref SystemState state) {
            foreach (var (moneyBuffer, moneyAmount)
                in SystemAPI.Query<DynamicBuffer<MoneyAddBufferElement>, RefRW<MoneyAmount>>()) {

                foreach (MoneyAddBufferElement money in moneyBuffer)
                    moneyAmount.ValueRW.Value += money.Value;

                moneyBuffer.Clear();
            }
        }
    }
}
