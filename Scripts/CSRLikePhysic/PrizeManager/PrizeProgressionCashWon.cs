public class PrizeProgressionCashWon : PrizeProgression
{
	public override string LocalisationTag
	{
		get
		{
			return "TEXT_PRIZE_PROGRESSION_CASH_WON";
		}
	}

	public override string FormatQuantity(float quantity)
	{
		return CurrencyUtils.GetCashString((int)quantity);
	}
}
