using DataSerialization;
using System;

public static class ThemeTransitionExtensions
{
	public static bool IsEligible(this ThemeTransition transition, IGameState gameState)
	{
		return transition.TransitionRequirements.IsEligible(gameState);
	}
}
