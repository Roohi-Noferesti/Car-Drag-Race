using System;
using I2.Loc;

namespace Objectives.Impl
{
	public class BeatXCrewMember : AbstractObjective
	{
		public string m_crewMemberToBeatAIName = string.Empty;

		public string m_crewMemberNickOveride = string.Empty;

		[SerializeInProfile]
		private int m_beatCount;

		public int m_beatTimes = 1;

		internal override void Clear()
		{
			base.Clear();
			this.m_beatCount = 0;
		}

		public override string GetDescription()
		{
			string arg = (!string.IsNullOrEmpty(this.m_crewMemberNickOveride)) ? LocalizationManager.GetTranslation(this.m_crewMemberNickOveride) : LocalizationManager.GetTranslation(this.m_crewMemberToBeatAIName);
			if (this.m_beatTimes > 1)
			{
				return string.Format(LocalizationManager.GetTranslation(this.Description), arg, this.m_beatTimes.ToString());
			}
			return string.Format(LocalizationManager.GetTranslation(this.Description), arg);
		}

        [Command]
		public void FinishdRaceWithEventInfo(RaceEventInfo raceEventInfo, bool isWinner)
		{
            if (raceEventInfo.AIDriverData != null && isWinner && raceEventInfo.AIDriverData.Name.ToUpper() == m_crewMemberToBeatAIName.ToUpper()) //LocalizationManager.GetTranslation(this.m_crewMemberToBeatAIName).ToUpper()))
            {
                this.m_beatCount++;
            }
		}

		public override void UpdateState()
		{
			if (RaceEventDatabase.instance.EventData == null || this.IsComplete)
			{
				return;
			}
			if (base.CurrentProgress < this.m_beatCount)
			{
				base.CurrentProgress = this.m_beatCount;
			}
			if (this.m_beatCount >= this.m_beatTimes)
			{
				base.ForceComplete();
			}
		}
	}
}
