using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace Assets.CodeBase.Vehicles.Wheels
{
    public enum WheelRotationType
    {
        None,
        Clockwise,
        Counterclockwise
    }

    public struct NewWheelTag : IComponentData { }
    public struct WheelInitializedTag : IComponentData { }
    public struct WheelHasGroundContactTag : IComponentData { }

    public struct WheelParent : IComponentData
    {
        public Entity Value;
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

    public struct WheelLinearVelocity : IComponentData
    {
        public float3 Value;
    }

    public struct WheelAxisProjectedVelocity : IComponentData
    {
        public float3 Value;
    }

    public struct WheelIndex : IComponentData
    {
        public int Value;
    }

    public struct WheelSteeringParameters : IComponentData
    {
        public float MaximalSteering;
        public float MinimalSteering;
    }

    public struct WheelSpringRestDistance : IComponentData
    {
        public float Value;
    }

    public struct WheelSpringCompression : IComponentData
    {
        public float Value;
    }

    public struct WheelSpringLengthCompressed : IComponentData
    {
        public float Value;
    }

    public struct WheelSpringStrength : IComponentData
    {
        public float Strength;
        public float Damper;
    }

    public struct WheelAxisForceSpring : IComponentData
    {
        public float3 Value;
    }

    public struct WheelAxisForceSteering : IComponentData
    {
        public float3 Value;
    }

    public struct WheelAxisForceAcceleration : IComponentData
    {
        public float3 Value;
    }

    public struct WheelBrakingTag : IComponentData { }
    public struct WheelAcceleratingTag : IComponentData { }
    public struct WheelHasAccelerationTag : IComponentData { }

    public struct WheelAccelerationInput : IComponentData
    {
        public float Value;
    }

    public struct WheelAccelerationParameters : IComponentData
    {
        public float MaxVelocity;
        public float MaxVelocityBackwards;
        public float EngineForceMultiplier;
        public float HardBrakingForceMultiplier;
    }

    public struct WheelHasRotationTag : IComponentData { }

    public struct WheelRotationInput : IComponentData
    {
        public float Value;
    }

    public struct WheelRotationParameters : IComponentData
    {
        public bool RotatesClockwise;
        public float MaxRotationAngle;
    }

    public struct WheelLatestForceApplyTick : IComponentData
    {
        public NetworkTick Value;
    }
}
