using DataSerialization;
using System;

public abstract class FormatStringModifier : StringModifier
{
	public abstract string[] ProvideFormattingArguments(IGameState gameState, StringModification.Details details);

	protected sealed override string Modify(string inputString, IGameState gameState, StringModification.Details details)
	{
		string[] array = this.ProvideFormattingArguments(gameState, details);
		if (array == null)
		{
			return null;
		}
		try
		{
			return string.Format(inputString, array);
		}
		catch (FormatException)
		{
		}
		return null;
	}
}
