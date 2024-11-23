using DataSerialization;
using System;
using System.Linq;

public static class ThemeAnimationsConfigurationExtensions
{
	public static ThemeAnimationDetail GetEligibleAnimation(this ThemeAnimationsConfiguration tac, IGameState gameState)
	{
		return tac.Animations.FirstOrDefault((ThemeAnimationDetail a) => a.IsEligible(gameState));
	}
}
