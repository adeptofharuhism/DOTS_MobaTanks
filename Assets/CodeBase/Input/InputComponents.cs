using Unity.Mathematics;
using Unity.NetCode;

namespace Assets.CodeBase.Input
{
    [GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
    public partial struct VehicleMovementInput : IInputComponentData
    {
        [GhostField(Quantization = 0)] public float2 Value;
    }
}
