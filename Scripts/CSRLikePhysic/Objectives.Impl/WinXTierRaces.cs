using System;
using I2.Loc;

namespace Objectives.Impl
{
	public class WinXTierRaces : AbstractWinRaces
	{
		[SerializeInProfile]
		private int m_winRaceCount;

		public eCarTier Tier;

		internal override void Clear()
		{
			base.Clear();
			this.m_winRaceCount = 0;
		}

		public override string GetDescription()
		{
			int num = (int)(this.Tier + 1);
			return string.Format(LocalizationManager.GetTranslation(this.Description), this.NumberOfRaces.ToString(), num.ToString());
		}

		[Command]
		public void FinishdRaceWithEventInfo(RaceEventInfo raceEventInfo, bool isWinner)
		{
			eCarTier currentEventTier = raceEventInfo.CurrentEventTier;
			if (currentEventTier == this.Tier && isWinner)
			{
				this.m_winRaceCount++;
			}
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
