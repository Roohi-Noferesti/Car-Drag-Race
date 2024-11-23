using System;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;

namespace Objectives.Impl
{
	public class WinXCrewCupRace : AbstractWinRaces
	{
		[SerializeInProfile]
		private int m_winRaceCount;

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
            //if (raceEventInfo.CurrentEvent != null && raceEventInfo.CurrentEvent.IsCrewEvent() && isWinner)
            //{
            //    this.m_winRaceCount++;
            //}
		}

		public override bool IsPossibleToComplete()
		{
		    //List<SeriesEvent> raceSeries = RaceEventDatabase.instance.InvitationalEvents.RaceSeries;
            //List<SeriesEvent> list = (from s in raceSeries
            //where s.IsSeriesCrewEvent && s.IsSeriesActive()
            //select s).ToList<SeriesEvent>();
            //return list.Count > 0;
		    return false;
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
