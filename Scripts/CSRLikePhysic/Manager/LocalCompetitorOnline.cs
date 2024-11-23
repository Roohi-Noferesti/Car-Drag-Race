using Metrics;
using System;
using System.Collections.Generic;

public abstract class LocalCompetitorOnline : LocalCompetitor
{
	protected override void OnEnteredPostRace()
	{
		base.OnEnteredPostRace();
		base.RecordReplayFinishTime();
		NetworkCompetitor networkCompetitor = CompetitorManager.Instance.OtherCompetitor as NetworkCompetitor;
		if (networkCompetitor == null)
		{
            return;
		}
		NetworkReplay playbackReplayData = networkCompetitor.PlaybackReplayData;
		if (!RaceResultsTracker.Them.TimeWasExtrapolated)
		{
			PlayerReplay playerReplay = new PlayerReplay(networkCompetitor.PlayerInfo, playbackReplayData.ReplayData);
			float num = Math.Abs(RaceResultsTracker.Them.RaceTime - networkCompetitor.PlaybackReplayData.ReplayData.finishTime);
			if (num >= 0.01f)
			{
				Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
				{
					{
						Parameters.ReplayUserID,
						NameValidater.CreateIdUsername(playerReplay.playerInfo.CsrUserID)
					},
					{
						Parameters.SimRaceTime,
						RaceResultsTracker.Them.RaceTime.ToString()
					},
					{
						Parameters.ReplayRaceTime,
						playbackReplayData.ReplayData.finishTime.ToString()
					},
					{
						Parameters.SimReplayRaceDeltaTime,
						num.ToString()
					},
					{
						Parameters.ReplayType,
						this.GetReplayType().ToString()
					}
				};
				Log.AnEvent(Events.ReplayVSSimulationTimeCheck, data);
			}
		}
		if (LocalCompetitor.useSimulationTime)
		{
			playbackReplayData.ReplayData.finishTime = RaceResultsTracker.Them.RaceTime;
		}
	}

	protected abstract ReplayType GetReplayType();
}
