using System;

public class DontShowOfferPopupData : BundleOfferPopupData
{
	public override bool IsEligible(IGameState gs)
	{
		return false;
	}
}
