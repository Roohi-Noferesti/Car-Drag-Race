public class PrizeProgressionFuelSpent : PrizeProgression
{
	public override string LocalisationTag
	{
		get
		{
			return "TEXT_PRIZE_PROGRESSION_FUEL_SPENT";
		}
	}

	public override string FormatQuantity(float quantity)
	{
		return string.Format("{0:#,###0}", (int)quantity);
	}
}
