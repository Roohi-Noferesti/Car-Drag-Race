using System;

namespace Objectives.Impl
{
	public class WinXLadderRacesInADay : AbstractWinRaces
	{
		[SerializeInProfile]
		private int m_winRaceCount;

	    [SerializeInProfile] private long m_resetDay;// = AbsoluteTime.AddAbsoluteTimeSeconds(AbsoluteTime.GetAbsoluteTimeNow(false), 86400L);

		private bool m_check = true;

		internal override void Clear()
		{
			base.Clear();
            this.m_winRaceCount = 0;
            m_resetDay = ServerSynchronisedTime.AddAbsoluteTimeSeconds(ServerSynchronisedTime.GetAbsoluteTimeNow(false), 86400L);
		}

		[Command]
		public void FinishdRaceWithEventInfo(RaceEventInfo raceEventInfo, bool isWinner)
		{
            if (raceEventInfo.CurrentEvent.IsLadderEvent() && isWinner)
            {
                this.m_winRaceCount++;
                ObjectiveManager.Instance.ForceUpdateObjectiveInProfile(this.ID);
            }
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

		public override void UpdateState()
		{
			if (RaceEventDatabase.instance.EventData == null || this.IsComplete)
			{
				return;
			}
			int num = 0;
			int num2 = 0;
			if (this.m_check)
			{
				for (eCarTier eCarTier = eCarTier.TIER_1; eCarTier < eCarTier.MAX_CAR_TIERS; eCarTier++)
				{
					BaseCarTierEvents tierEvents = RaceEventDatabase.instance.EventData.GetTierEvents(eCarTier);
					RaceEventData ladderEvent = RaceEventQuery.Instance.GetLadderEvent(tierEvents, false);
					if (ladderEvent != null)
					{
						num += ladderEvent.Group.NumOfEvents();
						num2 += ladderEvent.Group.NumEventsComplete();
					}
				}
				if (this.NumberOfRaces - this.m_winRaceCount > num - num2)
				{
					ObjectiveManager.Instance.SilentPassObjective(this);
					return;
				}
				this.m_check = false;
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
