using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DataSerialization;

public abstract class EligibilityConditionBase
{
	protected delegate bool IsStringMatchValidDelegate(string stringValue, IGameState gameState, EligibilityConditionDetails details);

	protected abstract bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details);

	public bool IsValid(IGameState gameState, EligibilityConditionDetails details)
	{
	    var res = this.IsConditionValid(gameState, details);
        return res == details.RequiredResult;
	}

	protected bool AreMatchesValid(List<string> stringValues, IsStringMatchValidDelegate isStringMatchValid, IGameState gameState, EligibilityConditionDetails details)
	{
		switch (details.MatchRequirmentEnum)
		{
		case EligibilityConditionDetails.ConditionMatchRequirment.Any:
			return stringValues.Any((string t) => isStringMatchValid(t, gameState, details));
		case EligibilityConditionDetails.ConditionMatchRequirment.All:
			return stringValues.All((string t) => isStringMatchValid(t, gameState, details));
		case EligibilityConditionDetails.ConditionMatchRequirment.Single:
			return stringValues.Count((string t) => isStringMatchValid(t, gameState, details)) == 1;
		default:
			return false;
		}
	}

	protected bool IsInRange(int value, EligibilityConditionDetails details)
	{
		return value >= details.MinValue && value <= details.MaxValue;
	}

	protected bool IsInRange(float value, EligibilityConditionDetails details)
	{
		return value >= details.MinFloatValue && value <= details.MaxFloatValue;
	}

	protected bool IsInRange(DateTime value, EligibilityConditionDetails details)
	{
		return value.CompareTo(details.MinDateTime) > 0 && value.CompareTo(details.MaxDateTime) < 0;
	}

	protected string WorldTourThemeID(IGameState gameState, EligibilityConditionDetails details)
	{
		if (string.IsNullOrEmpty(details.ThemeID))
		{
			return gameState.CurrentWorldTourThemeID;
		}
		return details.ThemeID.ToLower();
	}

	protected List<string> WorldTourThemeIDs(IGameState gameState, EligibilityConditionDetails details)
	{
		if (string.IsNullOrEmpty(details.ThemeID))
		{
			return new List<string>
			{
				gameState.CurrentWorldTourThemeID
			};
		}
		List<string> allThemeIDs = gameState.GetAllThemeIDs();
		return allThemeIDs.FindAll((string id) => Regex.IsMatch(id, details.ThemeID.ToLower()));
	}
}
