using DataSerialization;
using System;

public class RejectBundleOfferDataAction : PopupDataActionBase
{
	public override void Execute(EligibilityConditionDetails details)
	{
		if (BundleOfferController.Instance.TimerHasExpired())
		{
			PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
			bool isOfferRepeatable = BundleOfferController.Instance.ActiveOfferData.IsOfferRepeatable;
			activeProfile.SetPopupIsValid(details.IntValue, isOfferRepeatable);
			if (isOfferRepeatable)
			{
				int iD = BundleOfferController.Instance.ActiveOfferData.ID;
				activeProfile.QueuedOffersIDs.Remove(iD);
				activeProfile.ResetPopupData(iD);
			}
			activeProfile.LastBundleOfferTimeShown = GTDateTime.Now;
			BundleOfferController.Instance.DismissBubbleMessage();
			BundleOfferController.Instance.CleanUp();
			activeProfile.Save();
		}
		else
		{
			BundleOfferController.Instance.StartBubbleMessage();
			BundleOfferController.Instance.DisplayOfferIcon();
		}
	}
}
