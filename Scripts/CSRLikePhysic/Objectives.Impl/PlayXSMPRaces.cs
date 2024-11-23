using System;
using I2.Loc;

namespace Objectives.Impl
{
	public class PlayXSMPRaces : AbstractWinRaces
	{
		[SerializeInProfile]
		private int m_playRaceCount;

		internal override void Clear()
		{
			base.Clear();
			this.m_playRaceCount = 0;
		}

		[Command]
		public void FinishdRaceWithEventInfo(RaceEventInfo raceEventInfo, bool isWinner)
		{
            //if (raceEventInfo.IsSMPEvent || raceEventInfo.IsSMPBotRace)
            //{
            //    this.m_playRaceCount++;
            //}
		}

		public override string GetDescription()
		{
			return string.Format(LocalizationManager.GetTranslation(this.Description), this.NumberOfRaces.ToString());
		}

		public override void UpdateState()
		{
			if (RaceEventDatabase.instance.EventData == null || this.IsComplete)
			{
				return;
			}
			if (base.CurrentProgress < this.m_playRaceCount)
			{
				base.CurrentProgress = this.m_playRaceCount;
			}
			if (this.m_playRaceCount >= this.NumberOfRaces)
			{
				base.ForceComplete();
			}
		}
	}
}
