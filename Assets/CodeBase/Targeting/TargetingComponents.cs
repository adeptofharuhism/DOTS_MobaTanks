using Unity.Entities;

namespace Assets.CodeBase.Targeting
{
    public partial struct Targetable : IComponentData { }

    public partial struct TargetPoint : IComponentData
    {
        public Entity Value;
    }

    public partial struct Targeter : IComponentData { }

    public partial struct TargetOnCertainPointTag : IComponentData { }

    public enum TargetingType
    {
        Closest,
        Random
    }

    public partial struct TargeterRange : IComponentData
    {
        public float Value;
    }

    public partial struct CurrentTarget : IComponentData
    {
        public Entity Value;
    }
}
