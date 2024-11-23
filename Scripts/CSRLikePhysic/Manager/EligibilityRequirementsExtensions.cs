using DataSerialization;

public static class EligibilityRequirementsExtensions
{
	public static void Initialise(this EligibilityRequirements er)
	{
		er.PossibleGameStates.ForEach(delegate(EligibilityRequirements.PossibleGameState state)
		{
			state.Initialise();
		});
	}

	public static bool IsEligible(this EligibilityRequirements er, IGameState gameState)
	{
		foreach (EligibilityRequirements.PossibleGameState current in er.PossibleGameStates)
		{
			if (current.IsEligible(gameState))
			{
				return true;
			}
		}
		return false;
	}
}
