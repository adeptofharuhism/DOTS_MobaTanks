﻿using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Finances
{
    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct BasicMoneyAmount : IComponentData
    {
        public int Value;
    }

    public struct MoneyAmount : IComponentData
    {
        [GhostField] public int Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct MoneyAddBufferElement : IBufferElementData
    {
        public int Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct MoneyIncomeTag : IComponentData, IEnableableComponent { }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct MoneyIncome : IComponentData
    {
        public int Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct MoneyIncomeCooldown : IComponentData
    {
        public float Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Server)]
    public struct MoneyIncomeTimeLeft : IComponentData
    {
        public float Value;
    }
}
