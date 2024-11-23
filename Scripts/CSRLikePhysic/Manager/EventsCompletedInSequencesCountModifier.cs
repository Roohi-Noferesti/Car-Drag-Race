using DataSerialization;
using System;
using System.Linq;

public class EventsCompletedInSequencesCountModifier : FormatStringModifier
{
	public override string[] ProvideFormattingArguments(IGameState gameState, StringModification.Details details)
	{
		if (details.StringValues.Length % 2 != 0)
		{
			return null;
		}
		string[] array = details.StringValues.Where((string item, int index) => index % 2 == 0).ToArray<string>();
		int[] array2 = (from level in details.StringValues.Where((string item, int index) => index % 2 != 0)
		select int.Parse(level)).ToArray<int>();
		string themeID = details.ThemeID ?? gameState.CurrentWorldTourThemeID;
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			int num2 = gameState.LastWonEventSequenceLevel(themeID, array[i]);
			if (array2[i] <= num2)
			{
				num++;
			}
		}
		return new string[]
		{
			num.ToString()
		};
	}
}
