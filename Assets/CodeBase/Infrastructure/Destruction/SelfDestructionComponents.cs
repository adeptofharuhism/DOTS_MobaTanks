using Unity.Entities;

namespace Assets.CodeBase.Infrastructure.Destruction
{
    public partial struct DestroyEntityTag : IComponentData { }

    public partial struct SelfDestructLifetime : IComponentData
    {
        public float Value;
    }

    public partial struct SelfDestructCurrentTime : IComponentData
    {
        public float Value;
    }
}
