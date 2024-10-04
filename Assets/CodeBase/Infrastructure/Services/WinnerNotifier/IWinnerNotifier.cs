using Assets.CodeBase.Teams;
using System;

namespace Assets.CodeBase.Infrastructure.Services.WinnerNotifier
{
    public interface IWinnerNotifier
    {
        event Action<TeamType> OnWin;

        void NotifyWinnerTeam(TeamType winner);
    }
}
