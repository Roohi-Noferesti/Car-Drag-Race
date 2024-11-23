using System;
using I2.Loc;

namespace Objectives.Impl
{
    public class WinXCarSpecificRaces : AbstractWinRaces
	{
		[SerializeInProfile]
		private int m_winRaceCount;

		internal override void Clear()
		{
			base.Clear();
			this.m_winRaceCount = 0;
		}

		[Command]
		public void FinishdRaceWithEventInfo(RaceEventInfo raceEventInfo, bool isWinner)
		{
			if (raceEventInfo.CurrentEvent.IsCarSpecificEvent() && isWinner)
			{
				this.m_winRaceCount++;
			}
		}

		public override string GetDescription()
		{
			return string.Format(LocalizationManager.GetTranslation(this.Description), this.m_winRaceCount.ToString());
		}

		public override void UpdateState()
		{
			if (RaceEventDatabase.instance.EventData == null || this.IsComplete)
			{
				return;
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
