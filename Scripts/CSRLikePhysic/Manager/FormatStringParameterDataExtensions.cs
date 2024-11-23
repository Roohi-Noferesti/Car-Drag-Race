using DataSerialization;

public static class FormatStringParameterDataExtensions
{
	public enum DecorationType
	{
		NONE,
		GOLD_COLOUR,
		CASH_COLOUR
	}

	public static DecorationType GetDecorationEnum(this FormatStringParameterData fspd)
	{
		return EnumHelper.FromString<DecorationType>(fspd.DecorationString);
	}

	public static string GetDecoratedString(this FormatStringParameterData fspd)
	{
		switch (fspd.GetDecorationEnum())
		{
		case DecorationType.GOLD_COLOUR:
			return CurrencyUtils.GetColouredGoldString(int.Parse(fspd.Value));
		case DecorationType.CASH_COLOUR:
			return CurrencyUtils.GetColouredCashString(int.Parse(fspd.Value));
		}
		return fspd.Value;
	}
}
