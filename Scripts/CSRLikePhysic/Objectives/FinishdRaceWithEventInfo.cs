using System;

namespace Objectives
{
	public class FinishdRaceWithEventInfo : AbstractObjectiveCommand
	{
		public FinishdRaceWithEventInfo(RaceEventInfo raceEventInfo, bool isWinner) : base("FinishdRaceWithEventInfo", new object[]
		{
			raceEventInfo,
			isWinner
		})
		{
		}
	}
}
