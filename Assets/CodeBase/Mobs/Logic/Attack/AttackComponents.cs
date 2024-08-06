using Unity.Entities;

namespace Assets.CodeBase.Mobs.Logic.Attack
{
    public struct SquaredAttackDistance : IComponentData
    {
        public float Value;
    }

    public struct AttackIsReadyTag : IComponentData, IEnableableComponent { }
    public struct AttackIsOnCooldownTag : IComponentData, IEnableableComponent { }

    public struct AttackCooldown : IComponentData
    {
        public float Value;
    }

    public struct AttackCooldownTimeLeft : IComponentData
    {
        public float Value;
    }

    public struct AttackDamage : IComponentData
    {
        public float Value;
    }
}
