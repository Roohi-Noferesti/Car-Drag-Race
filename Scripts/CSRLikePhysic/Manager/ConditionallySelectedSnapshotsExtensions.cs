using DataSerialization;
using System;
using System.Collections.Generic;

public static class ConditionallySelectedSnapshotsExtensions
{
	public static void Initialise(this ConditionallySelectedSnapshots css)
	{
		css.Requirements.Initialise();
	}

	public static List<CarOverride> GetSnapshotList(this ConditionallySelectedSnapshots css, IGameState gameState)
	{
		if (css.Requirements.IsEligible(gameState))
		{
			return css.Cars;
		}
		return null;
	}
}
