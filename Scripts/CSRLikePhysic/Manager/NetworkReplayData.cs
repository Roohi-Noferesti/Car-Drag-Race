using System.Collections.Generic;

public class NetworkReplayData
{
	public float finishTime;

	public float finishSpeed;

	public List<ReplayEvent> GridReplayData = new List<ReplayEvent>(250);

	public List<ReplayEvent> RaceReplayData = new List<ReplayEvent>(1000);

	public string replayVersion;
}
