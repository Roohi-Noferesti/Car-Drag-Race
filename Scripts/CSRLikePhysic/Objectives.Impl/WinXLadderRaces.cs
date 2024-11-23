using System;
using I2.Loc;

namespace Objectives.Impl
{
	public class WinXLadderRaces : AbstractWinRaces
	{
		[SerializeInProfile]
		private int m_winRaceCount;

		private bool m_check = true;

		internal override void Clear()
		{
			base.Clear();
			this.m_winRaceCount = 0;
		}

		public override string GetDescription()
		{
			return string.Format(LocalizationManager.GetTranslation(this.Description), this.NumberOfRaces.ToString());
		}

		[Command]
		public void FinishdRaceWithEventInfo(RaceEventInfo raceEventInfo, bool isWinner)
		{
            if (raceEventInfo.CurrentEvent.IsLadderEvent() && isWinner)
            {
                this.m_winRaceCount++;
            }
		}

		private void CheckSkipForV1()
		{
			if (!this.IsPossibleToComplete())
			{
				ObjectiveManager.Instance.SilentPassObjective(this);
			}
		}

		public override bool IsPossibleToComplete()
		{
            int numberOfLadderevents = 0;
            int numberOfLaddereventsCompleted = 0;
			for (eCarTier eCarTier = eCarTier.TIER_1; eCarTier < eCarTier.MAX_CAR_TIERS; eCarTier++)
			{
				BaseCarTierEvents tierEvents = RaceEventDatabase.instance.EventData.GetTierEvents(eCarTier);
				RaceEventData ladderEvent = RaceEventQuery.Instance.GetLadderEvent(tierEvents, false);
				if (ladderEvent != null)
				{
					numberOfLadderevents += ladderEvent.Group.NumOfEvents();
                    numberOfLaddereventsCompleted += ladderEvent.Group.NumEventsComplete();
				}
			}
            return this.NumberOfRaces - this.m_winRaceCount <= numberOfLadderevents - numberOfLaddereventsCompleted;
		}

		public override void UpdateState()
		{
			if (RaceEventDatabase.instance.EventData == null || this.IsComplete)
			{
				return;
			}
			if (this.m_check && !ObjectiveManager.Instance.m_enableObjectivesV2)
			{
				this.CheckSkipForV1();
				this.m_check = false;
			}
			if (base.CurrentProgress < this.m_winRaceCount)
			{
				base.CurrentProgress = this.m_winRaceCount;
			}
			if (this.m_winRaceCount >= this.NumberOfRaces)
			{
				base.ForceComplete();
			}
		}
	}
}
