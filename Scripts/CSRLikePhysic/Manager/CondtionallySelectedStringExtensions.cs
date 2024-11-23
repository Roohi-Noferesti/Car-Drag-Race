using DataSerialization;
using System;
using System.Linq;

public static class CondtionallySelectedStringExtensions
{
	public static string GetText(this ConditionallySelectedString css, IGameState gameState)
	{
		return (from cms in css.Strings
		select cms.TextForGameState(gameState)).FirstOrDefault((string text) => text != null);
	}

	public static void Initialise(this ConditionallySelectedString css)
	{
		css.Strings.ForEach(delegate(ConditionallyModifiedString cs)
		{
			cs.Initialise();
		});
	}
}
