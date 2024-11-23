using System;

[Serializable]
public class SeasonEventMetadata
{
    public string EventTitle = string.Empty;

	public int ID = -1;

	public SeasonEventType EventType = SeasonEventType.Season;

	public int SeasonDisplayNumber = -1;

	public string MultiplayerBlurb = string.Empty;

	public string SeasonIntroText = string.Empty;

	public string SeasonInfoText = string.Empty;

	public string SeasonCarImageBundle = string.Empty;
}
