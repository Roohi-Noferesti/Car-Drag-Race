using System;
using KingKodeStudio;
using UnityEngine;

namespace Objectives.Impl
{
    public class OpenLeaderboardScreen : AbstractObjective
    {
        public void LeaderboardScreenOpened()
        {
            base.ForceComplete();
        }

        public override void UpdateState()
        {
            if (this.IsActive && !this.IsComplete && ScreenManager.Instance != null && ScreenManager.Instance.CurrentScreen==ScreenID.Leaderboards)
            {
                this.LeaderboardScreenOpened();
            }
        }
    }
}
