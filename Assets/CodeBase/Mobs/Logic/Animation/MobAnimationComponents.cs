using Unity.Entities;

namespace Assets.CodeBase.Mobs.Logic.Animation
{
    public struct AttackClipIndex : IComponentData
    {
        public byte Value;
    }

    public struct IdleClipIndex : IComponentData
    {
        public byte Value;
    }

    public struct RunClipIndex : IComponentData
    {
        public byte Value;
    }
}
