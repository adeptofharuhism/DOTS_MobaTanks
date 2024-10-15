using Assets.CodeBase.Teams;
using System;

namespace Assets.CodeBase.Infrastructure.Services.WorldEvents
{
    public interface IWorldEventBusService
    {
        event Action OnLoadedSubScene;
        event Action<TeamType> OnEndGame;
        event Action OnStartGame;
        event Action<int> OnUpdateMoneyAmount;
    }
}
