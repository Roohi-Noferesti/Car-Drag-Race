using I2.Loc;

public class PrizeProgressionTotalLeadTime : PrizeProgression
{
	public override string LocalisationTag
	{
		get
		{
			return "TEXT_PRIZE_PROGRESSION_TOTAL_LEAD_TIME";
		}
	}

	public override string FormatQuantity(float quantity)
	{
		return string.Format(LocalizationManager.GetTranslation("TEXT_UNITS_TIME_SECONDS_WITH_MS"), quantity);
	}
}
