public class PrizeProgressionPerfection : PrizeProgression
{
	public override string LocalisationTag
	{
		get
		{
			return "TEXT_PRIZE_PROGRESSION_PERFECTION";
		}
	}

	public override string FormatQuantity(float quantity)
	{
		return string.Format("{0:#,###0}", (int)quantity);
	}
}
