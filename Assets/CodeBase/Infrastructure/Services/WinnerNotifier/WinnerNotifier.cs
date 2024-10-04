using Assets.CodeBase.Teams;
using System;

namespace Assets.CodeBase.Infrastructure.Services.WinnerNotifier
{
    public class WinnerNotifier : IWinnerNotifier
    {
        public event Action<TeamType> OnWin;

        public void NotifyWinnerTeam(TeamType winner) =>
            OnWin?.Invoke(winner);
    }
}
