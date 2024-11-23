using System;

public class LocalCompetitorRTW : LocalCompetitorOnline
{
	protected override void OnEnteredPostRace()
	{
		base.OnEnteredPostRace();
        //NetworkCompetitor networkCompetitor = CompetitorManager.Instance.OtherCompetitor as NetworkCompetitor;
        //NetworkReplay playbackReplayData = networkCompetitor.PlaybackReplayData;
        PlayerReplay zOpponentPlayerReplay = null;// new PlayerReplay(networkCompetitor.PlayerInfo, playbackReplayData.ReplayData);
        PlayerReplay playerReplay = new PlayerReplay(base.PlayerInfo, this.RecordableReplayData.ReplayData);
        playerReplay.replayData.replayVersion = GameDatabase.Instance.OnlineConfiguration.NetworkReplayVersion;
        NetworkReplayManager.Instance.AddReplayToUploadQueue(playerReplay, zOpponentPlayerReplay, RaceEventInfo.Instance.CurrentEvent, ReplayType.RaceTheWorld);
		DifficultyManager.OnMultiplayerFinishedRace(RaceResultsTracker.You.RaceTime, RaceResultsTracker.You.IsWinner);
	}

	protected override ReplayType GetReplayType()
	{
		return ReplayType.RaceTheWorld;
	}
}
