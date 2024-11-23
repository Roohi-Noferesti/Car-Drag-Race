using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using KingKodeStudio;
using UnityEngine;

public class AgentUpgradeNags
{
	private static CarUpgradeData LastCashNag;

	public static void ResetLastUpgradeNags()
	{
		LastCashNag = FindBestUpgrade();
	}

	private static int GetHighestNaggableUpgradeLevel()
	{
		int highestNggableUpgradeLevel = 0;
		int num2 = 0;
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		foreach (eUpgradeType current in CarUpgrades.ValidUpgrades.Where(u=>u!=eUpgradeType.INTAKE))
		{
			int ownUpgradeLevel = activeProfile.GetUpgradeLevelOwned(current) - 1;
			if (ownUpgradeLevel > highestNggableUpgradeLevel)
			{
				highestNggableUpgradeLevel = ownUpgradeLevel;
				num2 = 0;
			}
			else if (ownUpgradeLevel == highestNggableUpgradeLevel)
			{
				num2++;
			}
		}
		if (num2 > 2)
		{
			highestNggableUpgradeLevel++;
		}
		return highestNggableUpgradeLevel;
	}

	public static bool IsUpgradeAvailable()
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tier1.LadderEvents.NumEventsComplete() >= 2 
            && !activeProfile.HasBoughtFirstUpgrade)
		{
			return false;
		}
		CarUpgradeData carUpgradeData = FindBestUpgrade();
		if (carUpgradeData == null)
		{
			return false;
		}
		if (ArrivalManager.Instance.isUpgradeOnOrder(activeProfile.GetCurrentCar().CarDBKey, carUpgradeData.UpgradeType))
		{
			return false;
		}
		LastCashNag = carUpgradeData;
		return true;
	}

    public static PopUp GetUpgradePopup()
    {
        string bodyText = string.Format(LocalizationManager.GetTranslation("TEXT_POPUPS_NAG_NEW_UPGRADE_BODY"),
            LocalizationManager.GetTranslation("TEXT_UPGRADES_" + CarUpgrades.UpgradeNames[LastCashNag.UpgradeType]));
        return new PopUp
        {
            Title = "TEXT_POPUPS_NAG_NEW_UPGRADE_TITLE",
            BodyText = bodyText,
            BodyAlreadyTranslated = true,
            IsBig = true,
            ConfirmAction = new PopUpButtonAction(OnUpgradeNag),
            CancelText = "TEXT_BUTTON_NO",
            ConfirmText = "TEXT_BUTTON_UPGRADE",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
            ImageCaption = "TEXT_NAME_AGENT"
        };
    }

    private static void OnUpgradeNag()
	{
		TuningScreen.ExternalStartScreenOn = LastCashNag.UpgradeType;
		TuningScreen.OfferMode = true;
        ScreenManager.Instance.PushScreen(ScreenID.Tuning);
	}

	private static bool ShouldNagSpacing(ref int TriesSinceLastNag, int NagSpacing)
	{
		TriesSinceLastNag++;
		if (TriesSinceLastNag > NagSpacing)
		{
			TriesSinceLastNag = 0;
			return true;
		}
		return false;
	}

	private static bool ShouldShowUpgradeTyres()
	{
		CarGarageInstance currentCar = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
		int num = Mathf.CeilToInt((float)(currentCar.UpgradeStatus[eUpgradeType.ENGINE].levelFitted + currentCar.UpgradeStatus[eUpgradeType.TURBO].levelFitted) / 2f);
		return num >= (int)(currentCar.UpgradeStatus[eUpgradeType.TYRES].levelOwned + 2);
	}

	public static bool TryNag_UpgradeTyres()
	{
		if (!ShouldShowUpgradeTyres())
		{
			return false;
		}
		PopUp popup = new PopUp
		{
			Title = "TEXT_POPUPS_NAG_WHEELSPIN_TITLE",
			BodyText = "TEXT_POPUPS_NAG_WHEELSPIN_BODY",
			IsBig = true,
			ConfirmText = "TEXT_BUTTON_OK",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
			ImageCaption = "TEXT_NAME_AGENT"
		};
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
		return true;
	}

    public static CarUpgradeData FindBestUpgrade()
    {
        int lowPrice = (AgentUpgradeNags.LastCashNag != null) ? AgentUpgradeNags.LastCashNag.CostInCash : 1;
        PlayerProfile profile = PlayerProfileManager.Instance.ActiveProfile;
        int currentCash = profile.GetCurrentCash();
        List<CarUpgradeData> list =
            CarDatabase.Instance.GetAllAvailableUpgradesForCar(profile.GetCurrentCar().CarDBKey)
                .Where(u => u.UpgradeType != eUpgradeType.INTAKE)
                .ToList();
        int maxLevelNaggable = AgentUpgradeNags.GetHighestNaggableUpgradeLevel();
        list =
            list.FindAll(
                (CarUpgradeData upg) =>
                    upg.CostInCash > lowPrice && upg.CostInCash <= currentCash &&
                    (int) upg.UpgradeLevel <= maxLevelNaggable &&
                    (int) upg.UpgradeLevel == profile.GetUpgradeLevelOwned(upg.UpgradeType)+1);
        CarUpgradeData bestUpgrade;
        if (list.Count > 0)
        {
            bestUpgrade = list.MaxItem((CarUpgradeData q) => q.CostInCash);
        }
        else
        {
            bestUpgrade = null;
        }
        return bestUpgrade;
    }
}
