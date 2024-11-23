using System.Collections.Generic;
using DataSerialization;

public class WorldTourLastRaceInSequenceWasFinalPinCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		List<string> stringValues = base.WorldTourThemeIDs(gameState, details);
		return base.AreMatchesValid(stringValues, new IsStringMatchValidDelegate(this.LastRaceWasFinalPin), gameState, details);
	}

	private bool LastRaceWasFinalPin(string theme, IGameState gameState, EligibilityConditionDetails details)
	{
		int num = gameState.LastRacedEventSequenceLevel(theme, details.StringValue);
		int eventCountInSequenceFromProfile = gameState.GetEventCountInSequenceFromProfile(theme, details.StringValue);
		return num == eventCountInSequenceFromProfile - 1;
	}
}
