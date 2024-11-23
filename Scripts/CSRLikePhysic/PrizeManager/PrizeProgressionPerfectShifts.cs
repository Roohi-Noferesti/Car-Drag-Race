public class PrizeProgressionPerfectShifts : PrizeProgression
{
	public override string LocalisationTag
	{
		get
		{
			return "TEXT_PRIZE_PROGRESSION_PERFECT_SHIFTS";
		}
	}

	public override string FormatQuantity(float quantity)
	{
		return string.Format("{0:#,###0}", (int)quantity);
	}
}
