using Unity.Entities;
using Unity.NetCode;

namespace Assets.CodeBase.Mobs.Logic.Animation
{
    public struct IsMoving : IComponentData
    {
        [GhostField] public bool Value;
    }

    public struct IsAttacking : IComponentData
    {
        [GhostField] public bool Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Client)]
    public struct PreviousIsMoving : IComponentData
    {
        public bool Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Client)]
    public struct PreviousIsAttacking : IComponentData
    {
        public bool Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Client)]
    public struct AttackClipIndex : IComponentData
    {
        public byte Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Client)]
    public struct IdleClipIndex : IComponentData
    {
        public byte Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.Client)]
    public struct RunClipIndex : IComponentData
    {
        public byte Value;
    }
}
