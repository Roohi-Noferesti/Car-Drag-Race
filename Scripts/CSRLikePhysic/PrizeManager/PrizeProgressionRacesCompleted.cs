public class PrizeProgressionRacesCompleted : PrizeProgression
{
	public override string LocalisationTag
	{
		get
		{
			return "TEXT_PRIZE_PROGRESSION_RACES_COMPLETED";
		}
	}

	public override string FormatQuantity(float quantity)
	{
		return string.Format("{0:#,###0}", (int)quantity);
	}
}
