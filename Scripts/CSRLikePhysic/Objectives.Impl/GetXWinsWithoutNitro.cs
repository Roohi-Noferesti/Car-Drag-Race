using System;

namespace Objectives.Impl
{
	public class GetXWinsWithoutNitro : AbstractWinRaces
	{
		[SerializeInProfile]
		private int m_winsWithoutNitro;

		internal override void Clear()
		{
			base.Clear();
			this.m_winsWithoutNitro = 0;
		}

		[Command]
		public void FinishdRaceWithEventInfo(RaceEventInfo raceEventInfo, bool isWinner)
        {
			if (!CompetitorManager.Instance.LocalCompetitor.CarPhysics.HasUsedNitrous && isWinner)
            {
                this.m_winsWithoutNitro++;
			}
		}

		public override void UpdateState()
		{
			if (RaceEventDatabase.instance.EventData == null || this.IsComplete)
			{
				return;
			}

            if (base.CurrentProgress < this.m_winsWithoutNitro)
			{
				base.CurrentProgress = this.m_winsWithoutNitro;
			}
			if (this.m_winsWithoutNitro >= this.NumberOfRaces)
			{
                base.ForceComplete();
            }
        }
	}
}
