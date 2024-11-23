using System;

namespace Objectives.Impl
{
	public class WinXLoanBattleInADay : AbstractWinRaces
	{
		[SerializeInProfile]
		private int m_winRaceCount;

	    [SerializeInProfile] private long m_resetDay;//= AbsoluteTime.AddAbsoluteTimeSeconds(AbsoluteTime.GetAbsoluteTimeNow(false), 86400L);

		internal override void Clear()
		{
			base.Clear();
            this.m_winRaceCount = 0;
            m_resetDay = ServerSynchronisedTime.AddAbsoluteTimeSeconds(ServerSynchronisedTime.GetAbsoluteTimeNow(false), 86400L);
		}

		public override TimeSpan GetTimeLimit()
		{
			TimeSpan timeSpan = TimeSpan.Zero;
            if (ObjectiveManager.Instance != null && ObjectiveManager.Instance.m_enableObjectivesV2)
            {
                timeSpan = base.GetTimeLimit();
            }
            else
            {
                timeSpan = ServerSynchronisedTime.GetAbsoluteRemaingTimeUntil(this.m_resetDay);
                if (timeSpan < TimeSpan.Zero)
                {
                    return TimeSpan.Zero;
                }
            }
			return timeSpan;
		}

		[Command]
		public void FinishdRaceWithEventInfo(RaceEventInfo raceEventInfo, bool isWinner)
		{
			if (raceEventInfo.IsDailyBattleEvent && isWinner)
			{
				this.m_winRaceCount++;
				ObjectiveManager.Instance.ForceUpdateObjectiveInProfile(this.ID);
			}
		}

		public override void UpdateState()
		{
			if (RaceEventDatabase.instance.EventData == null || this.IsComplete)
			{
				return;
			}
            if (this.m_resetDay <= ServerSynchronisedTime.GetAbsoluteTimeNow(false))
            {
                this.m_winRaceCount = 0;
                base.CurrentProgress = 0;
                this.m_resetDay = ServerSynchronisedTime.AddAbsoluteTimeSeconds(ServerSynchronisedTime.GetAbsoluteTimeNow(false), 86400L);
                ObjectiveManager.Instance.ForceUpdateObjectiveInProfile(this.ID);
            }
			if (base.CurrentProgress < this.m_winRaceCount)
			{
				base.CurrentProgress = this.m_winRaceCount;
			}
			if (this.m_winRaceCount == this.NumberOfRaces)
			{
				base.ForceComplete();
			}
		}
	}
}
