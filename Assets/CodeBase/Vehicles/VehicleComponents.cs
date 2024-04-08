using Unity.Entities;

namespace Assets.CodeBase.Vehicles
{
    public partial struct VehicleTag : IComponentData { }
    public partial struct NewVehicleTag : IComponentData { }
    public partial struct NotOwnerVehicleTag : IComponentData { }
    public partial struct OwnerVehicleTag : IComponentData { }
}
