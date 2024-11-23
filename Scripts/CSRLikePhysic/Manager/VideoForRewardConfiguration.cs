using AdSystem.Enums;
using System;

[Serializable]
public class VideoForRewardConfiguration
{
	public enum eRewardID
	{
		Invalid = 0,
		InterstitialForCash = 1,
		PressForFuel = 2,
		WatchForFuelPrompted = 3,
		WatchForStreakRescue = 4,
		VideoForRPBonus = 5,
		WatchToSkipDeliveryTime = 6,
		VideoForDoubledPrize = 7,
		VideoForBonusDailyPrize = 8,
		VideoForExtraCashPrize = 9,
		PartDelivery = 10,
		CarDelivery = 11,
		AppTuttiTimedReward = 12
	}

	public enum eRewardType
	{
		Invalid = 0,
		Fuel = 1,
		Cash = 2,
		Gold = 3,
		StreakRescue = 4,
		RPBonus = 5,
		ReduceDeliveryTime = 6,
		DoubledPrize = 7,
		ExtraCash = 8,
		AppTuttiTimedReward = 9
	}

	public bool Enabled;

	public eRewardID RewardID;

	public AdSpace AdSpace;

	public eRewardType RewardType;

	public int RewardAmountT1;

	public int RewardAmountT2;

	public int RewardAmountT3;

	public int RewardAmountT4;

	public int RewardAmountT5;

	public int RewardAmountTX;

	public bool EnablePreAdPrompt;

	public bool EnablePostAdPrompt;

	public string PreAdPromptTitleTextID;

	public string PreAdPromptBodyTextID;

	public string PreAdPromptOkButtonTextID;

	public string PreAdPromptCancelButtonTextID;

	public string PostAdPromptTitleTextID;

	public string PostAdPromptBodyTextID;

	public string PostAdPromptOkButtonTextID;

	public string VideoCapHitPromptTitleTextID;

	public string VideoCapHitPromptBodyTextID;

	public string VideoCapHitPromptOkButtonTextID;

	public bool EnableVideo24HCap;

	public int Video24HCap;

	public AdSpace GetAdSpace()
	{
        return AdSpace;
	}
}
