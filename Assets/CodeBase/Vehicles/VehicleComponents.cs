using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace Assets.CodeBase.Vehicles
{
    public partial struct VehicleTag : IComponentData { }
    public partial struct NewVehicleTag : IComponentData { }
    public partial struct NotOwnerVehicleTag : IComponentData { }
    public partial struct OwnerVehicleTag : IComponentData { }

    public partial struct VehicleMovementInput : IInputComponentData
    {
        [GhostField(Quantization = 0)] public float2 Value;
    }

    public struct VehicleWheelAmount : IComponentData
    {
        public int Value;
    }

    public struct VehicleItemSlot : IComponentData
    {
        public Entity Value;
    }
}
