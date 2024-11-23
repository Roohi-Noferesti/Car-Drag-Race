public class PrizeProgressionMilesDriven : PrizeProgression
{
	public override string LocalisationTag
	{
		get
		{
			return "TEXT_PRIZE_PROGRESSION_MILES_DRIVEN";
		}
	}

	public override string FormatQuantity(float quantity)
	{
		if (quantity % 1f == 0f)
		{
			return quantity.ToString();
		}
		if ((double)quantity % 0.5 == 0.0)
		{
			return string.Format("{0:0.0}", quantity);
		}
		return string.Format("{0:0.00}", quantity);
	}
}
