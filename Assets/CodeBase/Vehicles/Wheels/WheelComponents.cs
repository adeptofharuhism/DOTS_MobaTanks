using Unity.Entities;

namespace Assets.CodeBase.Vehicles.Wheels
{
    public enum WheelRotationType
    {
        None,
        Clockwise,
        Counterclockwise
    }

    public struct WheelHasGroundContact : IComponentData
    {
        public bool Value;
    }

    public struct WheelForceCastPoint : IComponentData
    {
        public Entity Value;
    }

    public struct WheelModelParameters : IComponentData
    {
        public Entity ModelContainer;
        public float Diameter;
    }

    public struct WheelIndex : IComponentData
    {
        public int Value;
    }

    public struct WheelSpringRestDistance : IComponentData
    {
        public float Value;
    }

    public struct WheelCompressedSpringLength : IComponentData
    {
        public float Value;
    }
}
