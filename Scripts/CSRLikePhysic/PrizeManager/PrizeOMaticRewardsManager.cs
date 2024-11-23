using System;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine;

public static class PrizeOMaticRewardsManager
{
	public enum DebugRewards
	{
		SportCarPart,
		DesiribleCarPart,
		CommonCarPart,
		FuelRefill,
		CashTiny,
		CashSmall,
		CashMedium,
		CashLarge,
		CashHuge,
		GoldTiny,
		GoldSmall,
		GoldMedium,
		GoldLarge,
		GoldHuge,
		PipsOfFuel,
		FreeUpgrade,
		EliteClubEntry,
		RPTiny,
		RPSmall,
		RPMedium,
		RPLarge,
		RPHuge,
		ProTuner,
		N20Maniac,
		TiresCrew
	}

	private const float COMMON_CAR_PART_1 = 0.66f;

	private const float OTHER_COMMON_CAR_PART = 0.333f;

	private const float DESIRABLE_CAR_PART_1 = 0.5f;

	private static int _PrizePoolSize;

	private static int _RunningTotal;

	private static bool _CanWinFuelPips;

	private static bool _PrizeIsReplacement;

	private static List<Reward> ListOfCarRewards = new List<Reward>
	{
		Reward.SportCarPart,
		Reward.DesiribleCarPart1,
		Reward.DesiribleCarPart2,
		Reward.CommonCarPart1,
		Reward.CommonCarPart2,
		Reward.CommonCarPart3
	};

	private static Dictionary<Reward, string> PrizePopUpTitleDic = new Dictionary<Reward, string>
	{
		{
			Reward.SportCarPart,
			"TEXT_PRIZE_CARPART"
		},
		{
			Reward.DesiribleCarPart1,
			"TEXT_PRIZE_CARPART"
		},
		{
			Reward.DesiribleCarPart2,
			"TEXT_PRIZE_CARPART"
		},
		{
			Reward.CommonCarPart1,
			"TEXT_PRIZE_CARPART"
		},
		{
			Reward.CommonCarPart2,
			"TEXT_PRIZE_CARPART"
		},
		{
			Reward.CommonCarPart3,
			"TEXT_PRIZE_CARPART"
		},
		{
			Reward.CashTiny,
			"TEXT_PRIZE_CASH"
		},
		{
			Reward.CashSmall,
			"TEXT_PRIZE_CASH"
		},
		{
			Reward.CashMedium,
			"TEXT_PRIZE_CASH"
		},
		{
			Reward.CashLarge,
			"TEXT_PRIZE_CASH"
		},
		{
			Reward.CashHuge,
			"TEXT_PRIZE_CASH"
		},
		{
			Reward.FreeUpgrade,
			"TEXT_PRIZE_UPGRADE"
		},
		{
			Reward.FuelRefill,
			"TEXT_PRIZE_FUELREFILL"
		},
		{
			Reward.GoldTiny,
			"TEXT_PRIZE_GOLD"
		},
		{
			Reward.GoldSmall,
			"TEXT_PRIZE_GOLD"
		},
		{
			Reward.GoldMedium,
			"TEXT_PRIZE_GOLD"
		},
		{
			Reward.GoldLarge,
			"TEXT_PRIZE_GOLD"
		},
		{
			Reward.GoldHuge,
			"TEXT_PRIZE_GOLD"
		},
		{
			Reward.PipsOfFuel,
			"TEXT_PRIZE_PIPS"
		},
		{
			Reward.RPTiny,
			"TEXT_PRIZE_RP"
		},
		{
			Reward.RPSmall,
			"TEXT_PRIZE_RP"
		},
		{
			Reward.RPMedium,
			"TEXT_PRIZE_RP"
		},
		{
			Reward.RPLarge,
			"TEXT_PRIZE_RP"
		},
		{
			Reward.RPHuge,
			"TEXT_PRIZE_RP"
		},
		{
			Reward.ProTuner,
			"TEXT_NAME_CONSUMABLES_ENGINE"
		},
		{
			Reward.N20Maniac,
			"TEXT_NAME_CONSUMABLES_NITROUS"
		},
		{
			Reward.TiresCrew,
			"TEXT_NAME_CONSUMABLES_TYRES"
		}
	};

	private static Dictionary<Reward, RewardSize> rewardToSize = new Dictionary<Reward, RewardSize>
	{
		{
			Reward.GoldTiny,
			RewardSize.Tiny
		},
		{
			Reward.GoldSmall,
			RewardSize.Small
		},
		{
			Reward.GoldMedium,
			RewardSize.Medium
		},
		{
			Reward.GoldLarge,
			RewardSize.Large
		},
		{
			Reward.GoldHuge,
			RewardSize.Huge
		},
		{
			Reward.CashTiny,
			RewardSize.Tiny
		},
		{
			Reward.CashSmall,
			RewardSize.Small
		},
		{
			Reward.CashMedium,
			RewardSize.Medium
		},
		{
			Reward.CashLarge,
			RewardSize.Large
		},
		{
			Reward.CashHuge,
			RewardSize.Huge
		},
		{
			Reward.RPTiny,
			RewardSize.Tiny
		},
		{
			Reward.RPSmall,
			RewardSize.Small
		},
		{
			Reward.RPMedium,
			RewardSize.Medium
		},
		{
			Reward.RPLarge,
			RewardSize.Large
		},
		{
			Reward.RPHuge,
			RewardSize.Huge
		}
	};

	public static Reward GetRandomReward(PrizeomaticRewards cardData)
	{
		PrizeOMaticRewardsManager._CanWinFuelPips = PrizeOMaticRewardsManager.CanWinFuelPips();
		PrizeOMaticRewardsManager._PrizeIsReplacement = false;
		PrizeOMaticRewardsManager._PrizePoolSize = PrizeOMaticRewardsManager.CalculateSizeOfPrizePool();
		if (PrizeOMaticRewardsManager._PrizePoolSize <= 0)
		{
			PrizeOMaticRewardsManager.RefreshPrizePool(cardData);
			PrizeOMaticRewardsManager._PrizePoolSize = PrizeOMaticRewardsManager.CalculateSizeOfPrizePool();
		}
		return PrizeOMaticRewardsManager.WhatPrizeDidYouWin(cardData);
	}

	private static bool CanWinFuelPips()
	{
	    return !UnlimitedFuelManager.IsActive && FuelManager.Instance.GetFuel() < 10;
	}

	public static bool PrizeIsReplacement()
	{
		return PrizeOMaticRewardsManager._PrizeIsReplacement;
	}

	public static int CalculateSizeOfPrizePool()
	{
		int num = 0;
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (PrizeOMaticRewardsManager._CanWinFuelPips)
		{
			num += activeProfile.NumberOfFuelPipsRewardsRemaining;
		}
		num += activeProfile.NumberOfSportsCarPiecesRemaining;
		num += activeProfile.NumberOfDesiribleCarPiecesRemaining;
		num += activeProfile.NumberOfCommonCarPiecesRemaining;
		num += activeProfile.NumberOfTinyCashRewardsRemaining;
		num += activeProfile.NumberOfSmallCashRewardsRemaining;
		num += activeProfile.NumberOfMediumCashRewardsRemaining;
		num += activeProfile.NumberOfLargeCashRewardsRemaining;
		num += activeProfile.NumberOfHugeCashRewardsRemaining;
		num += activeProfile.NumberOfTinyGoldRewardsRemaining;
		num += activeProfile.NumberOfSmallGoldRewardsRemaining;
		num += activeProfile.NumberOfMediumGoldRewardsRemaining;
		num += activeProfile.NumberOfLargeGoldRewardsRemaining;
		num += activeProfile.NumberOfHugeGoldRewardsRemaining;
		num += activeProfile.NumberOfUpgradeRewardsRemaining;
		num += activeProfile.NumberOfTinyRPRewardsRemaining;
		num += activeProfile.NumberOfSmallRPRewardsRemaining;
		num += activeProfile.NumberOfMediumRPRewardsRemaining;
		num += activeProfile.NumberOfLargeRPRewardsRemaining;
		num += activeProfile.NumberOfHugeRPRewardsReamining;
		num += activeProfile.NumberOfProTunerRewardsRemaining;
		num += activeProfile.NumberOfTireCrewRewardsRemaining;
		num += activeProfile.NumberOfN20ManiacRewardsRemaining;
		return num + activeProfile.NumberOfFuelRefillsRemaining;
	}

	private static bool DoYouWinThisPrize(int prizeProbality, int randomPrize)
	{
		if (prizeProbality <= 0)
		{
			return false;
		}
		PrizeOMaticRewardsManager._RunningTotal += prizeProbality;
		return randomPrize <= PrizeOMaticRewardsManager._RunningTotal;
	}

	private static Reward WhatPrizeDidYouWin(PrizeomaticRewards cardData)
	{
		int randomPrize = UnityEngine.Random.Range(0, PrizeOMaticRewardsManager._PrizePoolSize);
		PrizeOMaticRewardsManager._RunningTotal = 0;
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (PrizeOMaticRewardsManager.DoYouWinThisPrize(activeProfile.NumberOfFuelPipsRewardsRemaining, randomPrize))
		{
			if (PrizeOMaticRewardsManager._CanWinFuelPips)
			{
				return Reward.PipsOfFuel;
			}
			activeProfile.NumberOfFuelPipsRewardsRemaining--;
			PrizeOMaticRewardsManager._PrizeIsReplacement = true;
			return GameDatabase.Instance.Online.UnlimitedFuelFallbackPrize_Pips;
		}
		else
		{
			if (PrizeOMaticRewardsManager.DoYouWinThisPrize(activeProfile.NumberOfSportsCarPiecesRemaining, randomPrize))
			{
				return Reward.SportCarPart;
			}
			if (PrizeOMaticRewardsManager.DoYouWinThisPrize(activeProfile.NumberOfDesiribleCarPiecesRemaining, randomPrize))
			{
				if (UnityEngine.Random.value > 0.5f)
				{
					return Reward.DesiribleCarPart1;
				}
				return Reward.DesiribleCarPart2;
			}
			else if (PrizeOMaticRewardsManager.DoYouWinThisPrize(activeProfile.NumberOfCommonCarPiecesRemaining, randomPrize))
			{
				float value = UnityEngine.Random.value;
				if (value > 0.66f)
				{
					return Reward.CommonCarPart1;
				}
				if (value > 0.333f)
				{
					return Reward.CommonCarPart2;
				}
				return Reward.CommonCarPart3;
			}
			else if (PrizeOMaticRewardsManager.DoYouWinThisPrize(activeProfile.NumberOfUpgradeRewardsRemaining, randomPrize))
			{
				PrizeOMaticData prizeoMatic = GameDatabase.Instance.OnlineConfiguration.PrizeoMatic;
				int freeUpgradesLeft = PlayerProfileManager.Instance.ActiveProfile.FreeUpgradesLeft;
				if (freeUpgradesLeft < prizeoMatic.FreeUpgradesSavedLimit)
				{
					return Reward.FreeUpgrade;
				}
				activeProfile.NumberOfUpgradeRewardsRemaining--;
				PrizeOMaticRewardsManager._PrizeIsReplacement = true;
				if (PrizeOMaticRewardsManager._CanWinFuelPips && PlayerProfileManager.Instance.ActiveProfile.FuelPips < prizeoMatic.FreeUpgradesFallbackFuelTheshold)
				{
					return Reward.FuelRefill;
				}
				return prizeoMatic.FreeUpgradesFallback;
			}
			else if (PrizeOMaticRewardsManager.DoYouWinThisPrize(activeProfile.NumberOfFuelRefillsRemaining, randomPrize))
			{
				if (PrizeOMaticRewardsManager._CanWinFuelPips)
				{
					return Reward.FuelRefill;
				}
				activeProfile.NumberOfFuelRefillsRemaining--;
				PrizeOMaticRewardsManager._PrizeIsReplacement = true;
				return GameDatabase.Instance.Online.UnlimitedFuelFallbackPrize_Refill;
			}
			else
			{
				if (PrizeOMaticRewardsManager.DoYouWinThisPrize(activeProfile.NumberOfTinyGoldRewardsRemaining, randomPrize))
				{
					return Reward.GoldTiny;
				}
				if (PrizeOMaticRewardsManager.DoYouWinThisPrize(activeProfile.NumberOfSmallGoldRewardsRemaining, randomPrize))
				{
					return Reward.GoldSmall;
				}
				if (PrizeOMaticRewardsManager.DoYouWinThisPrize(activeProfile.NumberOfMediumGoldRewardsRemaining, randomPrize))
				{
					return Reward.GoldMedium;
				}
				if (PrizeOMaticRewardsManager.DoYouWinThisPrize(activeProfile.NumberOfLargeGoldRewardsRemaining, randomPrize))
				{
					return Reward.GoldLarge;
				}
				if (PrizeOMaticRewardsManager.DoYouWinThisPrize(activeProfile.NumberOfHugeGoldRewardsRemaining, randomPrize))
				{
					return Reward.GoldHuge;
				}
				if (PrizeOMaticRewardsManager.DoYouWinThisPrize(activeProfile.NumberOfTinyCashRewardsRemaining, randomPrize))
				{
					return Reward.CashTiny;
				}
				if (PrizeOMaticRewardsManager.DoYouWinThisPrize(activeProfile.NumberOfSmallCashRewardsRemaining, randomPrize))
				{
					return Reward.CashSmall;
				}
				if (PrizeOMaticRewardsManager.DoYouWinThisPrize(activeProfile.NumberOfMediumCashRewardsRemaining, randomPrize))
				{
					return Reward.CashMedium;
				}
				if (PrizeOMaticRewardsManager.DoYouWinThisPrize(activeProfile.NumberOfLargeCashRewardsRemaining, randomPrize))
				{
					return Reward.CashLarge;
				}
				if (PrizeOMaticRewardsManager.DoYouWinThisPrize(activeProfile.NumberOfHugeCashRewardsRemaining, randomPrize))
				{
					return Reward.CashHuge;
				}
				if (PrizeOMaticRewardsManager.DoYouWinThisPrize(activeProfile.NumberOfTinyRPRewardsRemaining, randomPrize))
				{
					return Reward.RPTiny;
				}
				if (PrizeOMaticRewardsManager.DoYouWinThisPrize(activeProfile.NumberOfSmallRPRewardsRemaining, randomPrize))
				{
					return Reward.RPSmall;
				}
				if (PrizeOMaticRewardsManager.DoYouWinThisPrize(activeProfile.NumberOfMediumRPRewardsRemaining, randomPrize))
				{
					return Reward.RPMedium;
				}
				if (PrizeOMaticRewardsManager.DoYouWinThisPrize(activeProfile.NumberOfLargeRPRewardsRemaining, randomPrize))
				{
					return Reward.RPLarge;
				}
				if (PrizeOMaticRewardsManager.DoYouWinThisPrize(activeProfile.NumberOfHugeRPRewardsReamining, randomPrize))
				{
					return Reward.RPHuge;
				}
				if (PrizeOMaticRewardsManager.DoYouWinThisPrize(activeProfile.NumberOfProTunerRewardsRemaining, randomPrize))
				{
					return Reward.ProTuner;
				}
				if (PrizeOMaticRewardsManager.DoYouWinThisPrize(activeProfile.NumberOfTireCrewRewardsRemaining, randomPrize))
				{
					return Reward.TiresCrew;
				}
				if (PrizeOMaticRewardsManager.DoYouWinThisPrize(activeProfile.NumberOfN20ManiacRewardsRemaining, randomPrize))
				{
					return Reward.N20Maniac;
				}
				return Reward.CashMedium;
			}
		}
	}

	public static void RefreshPrizePool(PrizeomaticRewards rewardData)
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		activeProfile.NumberOfSportsCarPiecesRemaining = rewardData.TotalNumOfSportsPieces;
		activeProfile.NumberOfDesiribleCarPiecesRemaining = rewardData.TotalNumOfDesiriblePieces;
		activeProfile.NumberOfCommonCarPiecesRemaining = rewardData.TotalNumOfCommomPieces;
		activeProfile.NumberOfUpgradeRewardsRemaining = rewardData.TotalNumOfFreeUpgrades;
		activeProfile.NumberOfTinyGoldRewardsRemaining = rewardData.TotalNumOfTinyGoldPrizes;
		activeProfile.NumberOfSmallGoldRewardsRemaining = rewardData.TotalNumOfSmallGoldPrizes;
		activeProfile.NumberOfMediumGoldRewardsRemaining = rewardData.TotalNumOfMediumGoldPrizes;
		activeProfile.NumberOfLargeGoldRewardsRemaining = rewardData.TotalNumOfLargeGoldPrizes;
		activeProfile.NumberOfHugeGoldRewardsRemaining = rewardData.TotalNumOfHugeGoldPrizes;
		activeProfile.NumberOfTinyCashRewardsRemaining = rewardData.TotalNumOfTinyCashPrizes;
		activeProfile.NumberOfSmallCashRewardsRemaining = rewardData.TotalNumOfSmallCashPrizes;
		activeProfile.NumberOfMediumCashRewardsRemaining = rewardData.TotalNumOfMediumCashPrizes;
		activeProfile.NumberOfLargeCashRewardsRemaining = rewardData.TotalNumOfLargeCashPrizes;
		activeProfile.NumberOfHugeCashRewardsRemaining = rewardData.TotalNumOfHugeCashPrizes;
		activeProfile.NumberOfFuelRefillsRemaining = rewardData.TotalNumOfFuelRefills;
		activeProfile.NumberOfFuelPipsRewardsRemaining = rewardData.TotalNumOfPipRefills;
		activeProfile.NumberOfTinyRPRewardsRemaining = rewardData.TotalNumOfTinyRPPrizes;
		activeProfile.NumberOfSmallRPRewardsRemaining = rewardData.TotalNumOfSmallRPPrizes;
		activeProfile.NumberOfMediumRPRewardsRemaining = rewardData.TotalNumOfMediumRPPrizes;
		activeProfile.NumberOfLargeRPRewardsRemaining = rewardData.TotalNumOfLargeRPPrizes;
		activeProfile.NumberOfHugeRPRewardsReamining = rewardData.TotalNumOfHugeRPPrizes;
		activeProfile.NumberOfProTunerRewardsRemaining = rewardData.TotalNumOfProTunerPrizes;
		activeProfile.NumberOfN20ManiacRewardsRemaining = rewardData.TotalNumOfN20MaiciacPrizes;
		activeProfile.NumberOfTireCrewRewardsRemaining = rewardData.TotalNumOfTireCrewPrizes;
	}

	public static bool IsRewardACarReward(Reward selectedReward)
	{
		return PrizeOMaticRewardsManager.ListOfCarRewards.Contains(selectedReward);
	}

	public static void DisplayAPrizePopup(Reward whichReward, string whichCar, PopUpButtonAction confirmAction)
	{
		string whichCar2 = (!string.IsNullOrEmpty(whichCar)) ? CarDatabase.Instance.GetCarNiceName(whichCar) : string.Empty;
		string prizePopUpBody = PrizeOMaticRewardsManager.GetPrizePopUpBody(whichReward, whichCar2);
		string prizePopUpTitle = PrizeOMaticRewardsManager.GetPrizePopUpTitle(whichReward);
		if (PrizeOMaticRewardsManager.IsRewardACarReward(whichReward))
		{
			whichCar = "NotACar";
		}
		PopUp popup = new PopUp
		{
			Title = prizePopUpTitle,
			TitleAlreadyTranslated = true,
			BodyText = prizePopUpBody,
			BodyAlreadyTranslated = true,
			ConfirmAction = confirmAction,
			ConfirmText = "TEXT_BUTTON_GETPRIZE",
            //IsCard = true,
			CardRewardType = whichReward,
			CarRewardCarDBKey = whichCar,
			DelayKillPopupByFrames = 1
		};
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
	}

	private static string GetPrizePopUpTitle(Reward whichReward)
	{
		if (!PrizeOMaticRewardsManager.PrizePopUpTitleDic.ContainsKey(whichReward))
		{
			return "TEXT_PRIZE_CARPART";
		}
		return LocalizationManager.GetTranslation(PrizeOMaticRewardsManager.PrizePopUpTitleDic[whichReward]);
	}

	private static string GetPrizePopUpBody(Reward whichReward, string whichCar)
	{
		switch (whichReward)
		{
		case Reward.SportCarPart:
			return string.Format(LocalizationManager.GetTranslation("TEXT_PRIZE_CARPART_BODY"), whichCar);
		case Reward.DesiribleCarPart1:
		case Reward.DesiribleCarPart2:
			return string.Format(LocalizationManager.GetTranslation("TEXT_PRIZE_CARPART_BODY"), whichCar);
		case Reward.CommonCarPart1:
		case Reward.CommonCarPart2:
		case Reward.CommonCarPart3:
			return string.Format(LocalizationManager.GetTranslation("TEXT_PRIZE_CARPART_BODY"), whichCar);
		case Reward.FuelRefill:
			return LocalizationManager.GetTranslation("TEXT_PRIZE_FUELREFILL_BODY");
		case Reward.CashTiny:
		case Reward.CashSmall:
		case Reward.CashMedium:
		case Reward.CashLarge:
		case Reward.CashHuge:
		{
			RewardSize cashSize = PrizeOMaticRewardsManager.rewardToSize[whichReward];
			int num = PrizeOMaticRewardsManager.CalculatePrizeomaticCashReward(cashSize);
		    string colouredCostStringBrief = CurrencyUtils.GetCashString(num);//  CurrencyUtils.GetColouredCostStringBrief(num, 0);
			PrizeProgression.AddProgress(PrizeProgressionType.CashWon, (float)num);
			return string.Format(LocalizationManager.GetTranslation("TEXT_PRIZE_CASH_BODY"), colouredCostStringBrief);
		}
		case Reward.GoldTiny:
		case Reward.GoldSmall:
		case Reward.GoldMedium:
		case Reward.GoldLarge:
		case Reward.GoldHuge:
		{
			RewardSize goldSize = PrizeOMaticRewardsManager.rewardToSize[whichReward];
			int zGoldCost = PrizeOMaticRewardsManager.CalculatePrizeomaticGoldReward(goldSize);
		    string colouredCostStringBrief2 = CurrencyUtils.GetGoldStringWithIcon(zGoldCost);// CurrencyUtils.GetColouredCostStringBrief(0, zGoldCost);
			return string.Format(LocalizationManager.GetTranslation("TEXT_PRIZE_GOLD_BODY"), colouredCostStringBrief2);
		}
		case Reward.PipsOfFuel:
		{
			int pipsOfFuelReward = GameDatabase.Instance.OnlineConfiguration.PrizeoMatic.PipsOfFuelReward;
			return string.Format(LocalizationManager.GetTranslation("TEXT_PRIZE_PIPS_BODY"), pipsOfFuelReward);
		}
		case Reward.FreeUpgrade:
			return LocalizationManager.GetTranslation("TEXT_PRIZE_UPGRADE_BODY");
		case Reward.RPTiny:
		case Reward.RPSmall:
		case Reward.RPMedium:
		case Reward.RPLarge:
		case Reward.RPHuge:
		{
			RewardSize rewardSize = PrizeOMaticRewardsManager.rewardToSize[whichReward];
            //RPReward rPCardReward = RPBonusManager.GetRPCardReward(rewardSize);
		    return string.Format(LocalizationManager.GetTranslation("TEXT_PRIZE_RP_BODY"), 0);//rPCardReward.Reward * 100f, rPCardReward.Duration);
		}
		case Reward.ProTuner:
		{
			int consumableExtensionDuration = GameDatabase.Instance.OnlineConfiguration.RaceTeamPrizeData.GetConsumableExtensionDuration(eCarConsumables.EngineTune);
			return string.Format(LocalizationManager.GetTranslation("TEXT_PRIZE_TUNER_BODY"), consumableExtensionDuration);
		}
		case Reward.N20Maniac:
		{
			int consumableExtensionDuration2 = GameDatabase.Instance.OnlineConfiguration.RaceTeamPrizeData.GetConsumableExtensionDuration(eCarConsumables.Nitrous);
			return string.Format(LocalizationManager.GetTranslation("TEXT_PRIZE_N20_BODY"), consumableExtensionDuration2);
		}
		case Reward.TiresCrew:
		{
			int consumableExtensionDuration3 = GameDatabase.Instance.OnlineConfiguration.RaceTeamPrizeData.GetConsumableExtensionDuration(eCarConsumables.Tyre);
			return string.Format(LocalizationManager.GetTranslation("TEXT_PRIZE_TIRE_BODY"), consumableExtensionDuration3);
		}
		default:
			return "A reward has been passed in which has not been recognised";
		}
	}

	public static int CalculatePrizeomaticCashReward(RewardSize cashSize)
	{
		PrizeOMaticData prizeoMatic = GameDatabase.Instance.OnlineConfiguration.PrizeoMatic;
		int cashRewardBase = prizeoMatic.CashRewardBase;
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		int currentPPIndex = activeProfile.GetCurrentCar().CurrentPPIndex;
		float num = StreakManager.GetMultiplierForPP(currentPPIndex);
        bool isRaceTheWorld = false;
        bool isOnlineRaceClub = false;
		if (RaceEventInfo.Instance.CurrentEvent != null)
		{
			isRaceTheWorld = RaceEventInfo.Instance.CurrentEvent.IsRaceTheWorldWorldTourEvent();
			isOnlineRaceClub = RaceEventInfo.Instance.CurrentEvent.IsOnlineClubRacingEvent();
		}
        if (isRaceTheWorld)
		{
			num *= prizeoMatic.WorldTourCashRewardMultiplier;
		}
        else if (isOnlineRaceClub)
		{
			num *= prizeoMatic.EliteCashRewardMultiplier;
		}
		switch (cashSize)
		{
		case RewardSize.Tiny:
			num *= prizeoMatic.CashRewardMultiplierTiny;
			break;
		case RewardSize.Small:
			num *= prizeoMatic.CashRewardMultiplierSmall;
			break;
		case RewardSize.Medium:
			num *= prizeoMatic.CashRewardMultiplierMedium;
			break;
		case RewardSize.Large:
			num *= prizeoMatic.CashRewardMultiplierLarge;
			break;
		case RewardSize.Huge:
			num *= prizeoMatic.CashRewardMultiplierHuge;
			break;
		}
		return (int)((float)cashRewardBase * num);
	}

    public static int CalculatePrizeomaticGoldReward(RewardSize goldSize)
    {
        PrizeOMaticData prizeoMatic = GameDatabase.Instance.OnlineConfiguration.PrizeoMatic;
        switch (goldSize)
        {
            case RewardSize.Tiny:
                return prizeoMatic.GoldRewardTiny;
            case RewardSize.Small:
                return prizeoMatic.GoldRewardSmall;
            case RewardSize.Medium:
                return prizeoMatic.GoldRewardMedium;
            case RewardSize.Large:
                return prizeoMatic.GoldRewardLarge;
            case RewardSize.Huge:
                return prizeoMatic.GoldRewardHuge;
            default:
                return 0;
        }
    }


    public static int CalculatePrizeomaticKeyReward(RewardSize keySize)
    {
        PrizeOMaticData prizeoMatic = GameDatabase.Instance.OnlineConfiguration.PrizeoMatic;
        switch (keySize)
        {
            case RewardSize.Tiny:
                return prizeoMatic.KeyRewardTiny;
            case RewardSize.Small:
                return prizeoMatic.KeyRewardSmall;
            case RewardSize.Medium:
                return prizeoMatic.KeyRewardMedium;
            case RewardSize.Large:
                return prizeoMatic.KeyRewardLarge;
            case RewardSize.Huge:
                return prizeoMatic.KeyRewardHuge;
            default:
                return 0;
        }
    }

    public static void CreateFakePool(PrizeomaticRewards rewardData, Dictionary<PrizeOMaticRewardsManager.DebugRewards, int> ListOfFakedPrizes)
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		activeProfile.NumberOfSportsCarPiecesRemaining = ((!ListOfFakedPrizes.ContainsKey(PrizeOMaticRewardsManager.DebugRewards.SportCarPart)) ? rewardData.TotalNumOfSportsPieces : ListOfFakedPrizes[PrizeOMaticRewardsManager.DebugRewards.SportCarPart]);
		activeProfile.NumberOfDesiribleCarPiecesRemaining = ((!ListOfFakedPrizes.ContainsKey(PrizeOMaticRewardsManager.DebugRewards.DesiribleCarPart)) ? rewardData.TotalNumOfDesiriblePieces : ListOfFakedPrizes[PrizeOMaticRewardsManager.DebugRewards.DesiribleCarPart]);
		activeProfile.NumberOfCommonCarPiecesRemaining = ((!ListOfFakedPrizes.ContainsKey(PrizeOMaticRewardsManager.DebugRewards.CommonCarPart)) ? rewardData.TotalNumOfCommomPieces : ListOfFakedPrizes[PrizeOMaticRewardsManager.DebugRewards.CommonCarPart]);
		activeProfile.NumberOfUpgradeRewardsRemaining = ((!ListOfFakedPrizes.ContainsKey(PrizeOMaticRewardsManager.DebugRewards.FreeUpgrade)) ? rewardData.TotalNumOfFreeUpgrades : ListOfFakedPrizes[PrizeOMaticRewardsManager.DebugRewards.FreeUpgrade]);
		activeProfile.NumberOfTinyGoldRewardsRemaining = ((!ListOfFakedPrizes.ContainsKey(PrizeOMaticRewardsManager.DebugRewards.GoldTiny)) ? rewardData.TotalNumOfTinyGoldPrizes : ListOfFakedPrizes[PrizeOMaticRewardsManager.DebugRewards.GoldTiny]);
		activeProfile.NumberOfSmallGoldRewardsRemaining = ((!ListOfFakedPrizes.ContainsKey(PrizeOMaticRewardsManager.DebugRewards.GoldSmall)) ? rewardData.TotalNumOfSmallGoldPrizes : ListOfFakedPrizes[PrizeOMaticRewardsManager.DebugRewards.GoldSmall]);
		activeProfile.NumberOfMediumGoldRewardsRemaining = ((!ListOfFakedPrizes.ContainsKey(PrizeOMaticRewardsManager.DebugRewards.GoldMedium)) ? rewardData.TotalNumOfMediumGoldPrizes : ListOfFakedPrizes[PrizeOMaticRewardsManager.DebugRewards.GoldMedium]);
		activeProfile.NumberOfLargeGoldRewardsRemaining = ((!ListOfFakedPrizes.ContainsKey(PrizeOMaticRewardsManager.DebugRewards.GoldLarge)) ? rewardData.TotalNumOfLargeGoldPrizes : ListOfFakedPrizes[PrizeOMaticRewardsManager.DebugRewards.GoldLarge]);
		activeProfile.NumberOfHugeGoldRewardsRemaining = ((!ListOfFakedPrizes.ContainsKey(PrizeOMaticRewardsManager.DebugRewards.GoldHuge)) ? rewardData.TotalNumOfHugeGoldPrizes : ListOfFakedPrizes[PrizeOMaticRewardsManager.DebugRewards.GoldHuge]);
		activeProfile.NumberOfTinyCashRewardsRemaining = ((!ListOfFakedPrizes.ContainsKey(PrizeOMaticRewardsManager.DebugRewards.CashTiny)) ? rewardData.TotalNumOfTinyCashPrizes : ListOfFakedPrizes[PrizeOMaticRewardsManager.DebugRewards.CashTiny]);
		activeProfile.NumberOfSmallCashRewardsRemaining = ((!ListOfFakedPrizes.ContainsKey(PrizeOMaticRewardsManager.DebugRewards.CashSmall)) ? rewardData.TotalNumOfSmallCashPrizes : ListOfFakedPrizes[PrizeOMaticRewardsManager.DebugRewards.CashSmall]);
		activeProfile.NumberOfMediumCashRewardsRemaining = ((!ListOfFakedPrizes.ContainsKey(PrizeOMaticRewardsManager.DebugRewards.CashMedium)) ? rewardData.TotalNumOfMediumCashPrizes : ListOfFakedPrizes[PrizeOMaticRewardsManager.DebugRewards.CashMedium]);
		activeProfile.NumberOfLargeCashRewardsRemaining = ((!ListOfFakedPrizes.ContainsKey(PrizeOMaticRewardsManager.DebugRewards.CashLarge)) ? rewardData.TotalNumOfLargeCashPrizes : ListOfFakedPrizes[PrizeOMaticRewardsManager.DebugRewards.CashLarge]);
		activeProfile.NumberOfHugeCashRewardsRemaining = ((!ListOfFakedPrizes.ContainsKey(PrizeOMaticRewardsManager.DebugRewards.CashHuge)) ? rewardData.TotalNumOfHugeCashPrizes : ListOfFakedPrizes[PrizeOMaticRewardsManager.DebugRewards.CashHuge]);
		activeProfile.NumberOfFuelRefillsRemaining = ((!ListOfFakedPrizes.ContainsKey(PrizeOMaticRewardsManager.DebugRewards.FuelRefill)) ? rewardData.TotalNumOfFuelRefills : ListOfFakedPrizes[PrizeOMaticRewardsManager.DebugRewards.FuelRefill]);
		activeProfile.NumberOfFuelPipsRewardsRemaining = ((!ListOfFakedPrizes.ContainsKey(PrizeOMaticRewardsManager.DebugRewards.PipsOfFuel)) ? rewardData.TotalNumOfPipRefills : ListOfFakedPrizes[PrizeOMaticRewardsManager.DebugRewards.PipsOfFuel]);
		activeProfile.NumberOfN20ManiacRewardsRemaining = ((!ListOfFakedPrizes.ContainsKey(PrizeOMaticRewardsManager.DebugRewards.N20Maniac)) ? rewardData.TotalNumOfN20MaiciacPrizes : ListOfFakedPrizes[PrizeOMaticRewardsManager.DebugRewards.N20Maniac]);
		activeProfile.NumberOfProTunerRewardsRemaining = ((!ListOfFakedPrizes.ContainsKey(PrizeOMaticRewardsManager.DebugRewards.ProTuner)) ? rewardData.TotalNumOfProTunerPrizes : ListOfFakedPrizes[PrizeOMaticRewardsManager.DebugRewards.ProTuner]);
		activeProfile.NumberOfTireCrewRewardsRemaining = ((!ListOfFakedPrizes.ContainsKey(PrizeOMaticRewardsManager.DebugRewards.TiresCrew)) ? rewardData.TotalNumOfTireCrewPrizes : ListOfFakedPrizes[PrizeOMaticRewardsManager.DebugRewards.TiresCrew]);
	}

    public static bool isRaceTheWorld { get; set; }

    public static bool isOnlineRaceClub { get; set; }
}
