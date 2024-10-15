using Unity.Entities;

namespace Assets.CodeBase.Infrastructure.Services.WorldAccess
{
    public class WorldAccessService : IWorldAccessService
    {
        public World DefaultWorld {
            get => World.DefaultGameObjectInjectionWorld;
            set => World.DefaultGameObjectInjectionWorld = value;
        }

        public WorldAccessService() { }
    }
}
