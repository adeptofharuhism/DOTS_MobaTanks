using Assets.CodeBase.Teams;
using Assets.CodeBase.Utility;
using System;

namespace Assets.CodeBase.Infrastructure.Services.WorldEvents
{
    public interface IWorldEventBusService
    {
        IReactiveGetter<bool> ShopAvailability { get; }
        IReactiveGetter<int> MoneyAmount { get; }

        event Action OnLoadedSubScene;
        event Action<TeamType> OnEndGame;
        event Action OnStartGame;
    }
}
