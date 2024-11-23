using DataSerialization;
using System;

public class WorldTourChoiceStringModifier : FormatStringModifier
{
	public override string[] ProvideFormattingArguments(IGameState gameState, StringModification.Details details)
	{
		string themeID = details.ThemeID ?? gameState.CurrentWorldTourThemeID;
		if (details.StringValues.Length != 2)
		{
			return null;
		}
		string sequenceID = details.StringValues[0];
		string pinID = details.StringValues[1];
		return new string[]
		{
			gameState.ChoiceSelection(themeID, sequenceID, pinID).ToString()
		};
	}
}
