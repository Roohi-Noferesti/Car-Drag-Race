using DataSerialization;
using System;

public static class ThemeProgressionDataExtensions
{
	public static void Initialise(this ThemeProgressionData tpd)
	{
		tpd.IntroNarrative.Initialise();
		tpd.IncreaseThemeCompletionLevelRequirements.Initialise();
	}
}
