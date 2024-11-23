public class PrizeProgressionGachaCards : PrizeProgression
{
	public override string LocalisationTag
	{
		get
		{
			return "TEXT_PRIZE_PROGRESSION_GACHA_CARDS";
		}
	}

	public override string FormatQuantity(float quantity)
	{
		return string.Format("{0:#,###0}", (int)quantity);
	}
}
