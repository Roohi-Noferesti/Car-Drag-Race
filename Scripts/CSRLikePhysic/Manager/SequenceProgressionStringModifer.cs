using DataSerialization;
using System;

public class SequenceProgressionStringModifer : FormatStringModifier
{
	public override string[] ProvideFormattingArguments(IGameState gameState, StringModification.Details details)
	{
		if (details.StringValue == null)
		{
			return null;
		}
		string themeID = details.ThemeID ?? gameState.CurrentWorldTourThemeID;
		string stringValue = details.StringValue;
		int num = gameState.GetEventCountInSequenceFromProfile(themeID, stringValue);
		if (num == 2147483647)
		{
			PinScheduleConfiguration pinSchedule = TierXManager.Instance.PinSchedule;
			if (pinSchedule != null)
			{
				num = pinSchedule.GetSequence(stringValue).Pins.Count;
			}
		}
		return new string[]
		{
			(gameState.LastWonEventSequenceLevel(themeID, stringValue) + details.Offset).ToString(),
			num.ToString()
		};
	}
}
