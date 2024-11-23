using System.Linq;
using DataSerialization;

public static class PossibleGameStateExtensions
{
	public static void Initialise(this EligibilityRequirements.PossibleGameState pgs)
	{
		pgs.Conditions.ForEach(delegate(EligibilityCondition condition)
		{
			condition.Initialise();
		});
	}

	public static bool IsEligible(this EligibilityRequirements.PossibleGameState pgs, IGameState gameState)
	{
		foreach (EligibilityCondition current in pgs.Conditions.Where(c=>c.IsActive))
		{
			if (!current.IsValid(gameState))
			{
				return false;
			}
		}
		return true;
	}
}
