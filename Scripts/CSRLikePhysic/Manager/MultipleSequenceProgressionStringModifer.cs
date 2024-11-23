using DataSerialization;
using System;
using System.Linq;

public class MultipleSequenceProgressionStringModifer : FormatStringModifier
{
	public override string[] ProvideFormattingArguments(IGameState gameState, StringModification.Details details)
	{
		if (details.StringValues == null || details.StringValues.Length == 0)
		{
			return null;
		}
		return new string[]
		{
			this.GetNumEventsWon(gameState, details),
			this.GetNumEventsOrDefault(gameState, details)
		};
	}

	private string GetNumEventsWon(IGameState gameState, StringModification.Details details)
	{
		string theme = details.ThemeID ?? gameState.CurrentWorldTourThemeID;
		return details.StringValues.Sum((string s) => gameState.LastWonEventSequenceLevel(theme, s) + details.Offset).ToString();
	}

	private string GetNumEventsOrDefault(IGameState gameState, StringModification.Details details)
	{
		string text = details.ThemeID ?? gameState.CurrentWorldTourThemeID;
		int num = 0;
		string[] stringValues = details.StringValues;
		for (int i = 0; i < stringValues.Length; i++)
		{
			string sequenceID = stringValues[i];
			int eventCountInSequenceFromProfile = gameState.GetEventCountInSequenceFromProfile(text, sequenceID);
			if (eventCountInSequenceFromProfile == 2147483647)
			{
				if (TierXManager.Instance.PinSchedule.themeID != text)
				{
					return (details.DefaultModifications == null || details.DefaultModifications.Length != 1) ? "0" : details.DefaultModifications[0];
				}
				PinScheduleConfiguration pinSchedule = TierXManager.Instance.PinSchedule;
				if (pinSchedule != null)
				{
					num += pinSchedule.GetSequence(sequenceID).Pins.Count;
				}
			}
			else
			{
				num += eventCountInSequenceFromProfile;
			}
		}
		return num.ToString();
	}
}
