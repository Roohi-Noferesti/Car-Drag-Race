using System.Collections.Generic;
using I2.Loc;
using Metrics;

public class OfferWallManager
{
	private class DummyOfferWallInterface : IOfferWallProvider
	{
		public void FetchContent()
		{
		}

		public void CheckForRewards()
		{
		}

		public bool ShowContent()
		{
			return false;
		}
	}

	private static IOfferWallProvider _tapjoyInterface;

	private static IOfferWallProvider _defaultInterface;

	private static IOfferWallProvider Provider
	{
		get
		{
			OfferWallConfiguration.eProvider offerWallProvider = GameDatabase.Instance.Ad.GetOfferWallProvider();
			if (offerWallProvider == OfferWallConfiguration.eProvider.TapJoy)
			{
				return _tapjoyInterface;
			}
			return _defaultInterface;
		}
	}

	public static void Initialise()
	{
	    _tapjoyInterface = null;//new TapJoyOfferwallInterface();
		_defaultInterface = new DummyOfferWallInterface();
		ApplicationManager.DidBecomeActiveEvent += CheckForReward;
	}

	public static void CheckForReward()
	{
		Provider.CheckForRewards();
	}

	public static void FetchContent()
	{
		Provider.FetchContent();
	}

	public static bool Show()
	{
		return Provider.ShowContent();
	}

	public static void AwardGold(int gold)
	{
		if (gold > 0)
		{
			string textID;
			if (gold == 1)
			{
				textID = "TEXT_POPUPS_OFFER_WALL_RECEIVED_GOLD_SINGULAR";
			}
			else
			{
				textID = "TEXT_POPUPS_OFFER_WALL_RECEIVED_GOLD_PLURAL";
			}
			string bodyText = string.Format(LocalizationManager.GetTranslation(textID), gold);
			PopUp popup = new PopUp
			{
				Title = "TEXT_POPUPS_OFFER_WALL_TITLE",
				BodyText = bodyText,
				IsBig = true,
				BodyAlreadyTranslated = true,
				ConfirmText = "TEXT_BUTTON_OK",
                //BundledGraphicPath = PopUpManager.Instance.graphics_agentPrefab,
				ImageCaption = "TEXT_NAME_AGENT"
			};
			PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
			SaveToProfile(gold);
			SendAwardGoldMetricEvent(gold);
		}
	}

	private static void SaveToProfile(int gold)
	{
		OfferWallConfiguration.eProvider offerWallProvider = GameDatabase.Instance.Ad.GetOfferWallProvider();
		PlayerProfileManager.Instance.ActiveProfile.AddOfferWallGold(offerWallProvider, gold);
		PlayerProfileManager.Instance.ActiveProfile.AddOfferWallEvent(offerWallProvider, 1);
		PlayerProfileManager.Instance.ActiveProfile.AddGold(gold, "OfferWallManager","OfferWallManager");
		PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
	}

	private static void SendAwardGoldMetricEvent(int gold)
	{
		Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
		{
			{
				Parameters.DGld,
				gold.ToString()
			},
			{
				Parameters.OWPvr,
				GameDatabase.Instance.Ad.GetOfferWallProvider().ToString()
			},
			{
				Parameters.baid,
				UserManager.Instance.currentAccount.Username
			}
		};
		Log.AnEvent(Events.OfferWallAwardGold, data);
	}
}
