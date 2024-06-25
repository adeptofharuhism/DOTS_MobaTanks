using Unity.Entities;

namespace Assets.CodeBase.Targeting
{
    public partial struct Targetable : IComponentData { }

    public partial struct TargetPosition : IComponentData
    {
        public Entity Value;
    }

    public partial struct Targeter : IComponentData { }

    public partial struct TargeterRange : IComponentData
    {
        public float Value;
    }

    public partial struct CurrentTarget : IComponentData
    {
        public Entity Value;
    }
}
