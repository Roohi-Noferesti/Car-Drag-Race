using System.Collections.Generic;
using DataSerialization;

public class HasJustFinishedWorldTourSequenceCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		foreach (string current in details.StringValues)
		{
			EligibilityRequirements.PossibleGameState pgs = new EligibilityRequirements.PossibleGameState
			{
				Conditions = new List<EligibilityCondition>
				{
					new EligibilityCondition
					{
						Type = "WorldTourSequenceComplete",
						Details = new EligibilityConditionDetails
						{
							StringValue = current,
							ThemeID = details.ThemeID.ToLower()
						}
					},
					new EligibilityCondition
					{
						Type = "CurrentWorldTourSequence",
						Details = new EligibilityConditionDetails
						{
							StringValue = current,
							ThemeID = details.ThemeID.ToLower()
						}
					}
				}
			};
			if (pgs.IsEligible(gameState))
			{
				return true;
			}
		}
		return false;
	}
}
