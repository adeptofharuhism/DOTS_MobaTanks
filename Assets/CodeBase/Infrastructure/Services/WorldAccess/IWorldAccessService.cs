using Unity.Entities;

namespace Assets.CodeBase.Infrastructure.Services.WorldAccess
{
    public interface IWorldAccessService
    {
        World DefaultWorld { get; set; }
    }
}
