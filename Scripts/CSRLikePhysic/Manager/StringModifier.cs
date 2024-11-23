using DataSerialization;
using System;
using I2.Loc;

public abstract class StringModifier
{
	protected abstract string Modify(string inputString, IGameState gameState, StringModification.Details details);

	public string Modify(IGameState gameState, StringModification.Details details)
	{
		string text = details.Default;
		if (details.Translate)
		{
			text = LocalizationManager.GetTranslation(text);
		}
		return this.Modify(text, gameState, details);
	}
}
