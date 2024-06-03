using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace Assets.CodeBase.Vehicles
{
    public partial struct VehicleTag : IComponentData { }
    public partial struct NewVehicleTag : IComponentData { }
    public partial struct NotOwnerVehicleTag : IComponentData { }
    public partial struct OwnerVehicleTag : IComponentData { }

    [GhostComponent(PrefabType = GhostPrefabType.All)]
    public partial struct VehicleMovementInput : IInputComponentData
    {
        [GhostField(Quantization = 0)] public float2 Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.All)]
    public partial struct VehicleSpringLengthCompressedBuffer : IBufferElementData
    {
        [GhostField(Quantization = 0)] public float Value;
        [GhostField] public int Index;
    }

    public struct VehicleWheelAmount : IComponentData
    {
        public int Value;
    }
}
