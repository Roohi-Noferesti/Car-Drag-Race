using DataSerialization;
using System;

public static class ConditonallyModifiedStringExtensions
{
	public static string TextForGameState(this ConditionallyModifiedString cms, IGameState gameState)
	{
		if (cms.Requirements.IsEligible(gameState))
		{
			return cms.StringModification.Modify(gameState);
		}
		return null;
	}

	public static void Initialise(this ConditionallyModifiedString cms)
	{
		cms.Requirements.Initialise();
	}
}
