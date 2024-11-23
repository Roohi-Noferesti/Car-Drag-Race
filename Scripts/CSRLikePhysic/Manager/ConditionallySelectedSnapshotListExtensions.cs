using DataSerialization;
using System;
using System.Collections.Generic;
using System.Linq;

public static class ConditionallySelectedSnapshotListExtensions
{
	public static void Initialise(this ConditionallySelectedSnapshotList cssl)
	{
		cssl.Lists.ForEach(delegate(ConditionallySelectedSnapshots cs)
		{
			cs.Initialise();
		});
	}

	public static List<CarOverride> GetSnapshotList(this ConditionallySelectedSnapshotList cssl, IGameState gameState)
	{
		return (from cms in cssl.Lists
		select cms.GetSnapshotList(gameState)).FirstOrDefault((List<CarOverride> cars) => cars != null);
	}
}
