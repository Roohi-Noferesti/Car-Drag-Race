using System;

public abstract class SpotPrize
{
	public abstract string GetPinDescription(SpotPrizeDetails details);

	public abstract void AwardPrize(SpotPrizeDetails details);

	public abstract string GetPopupBody(SpotPrizeDetails details);
}
