using DataSerialization;
using System;

public class AlwaysDefaultStringModifier : StringModifier
{
	protected override string Modify(string inputString, IGameState gameState, StringModification.Details details)
	{
		return inputString;
	}
}
