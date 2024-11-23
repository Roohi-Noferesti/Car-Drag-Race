using DataSerialization;
using System;

public class CurrentEventGroupOppenentNameModifier : FormatStringModifier
{
	public override string[] ProvideFormattingArguments(IGameState gameState, StringModification.Details details)
	{
		int offset = details.Offset;
		if (offset < 0 || offset >= RaceEventInfo.Instance.CurrentEvent.Group.RaceEvents.Count)
		{
			return null;
		}
		string aIDriver = RaceEventInfo.Instance.CurrentEvent.Group.RaceEvents[offset].AIDriver;
		string displayName = GameDatabase.Instance.AIPlayers.GetAIDriverData(aIDriver).GetDisplayName();
		return new string[]
		{
			displayName
		};
	}
}
