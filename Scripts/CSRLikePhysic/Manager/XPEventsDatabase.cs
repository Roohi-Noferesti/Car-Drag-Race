using System.Collections.Generic;
using UnityEngine;

public class XPEventsDatabase : ConfigurationAssetLoader
{
	public const int maxPlayerLevel = 999;

	private List<int> _xpPerLevel = new List<int>();

	private List<int> _xpTotalAtEndOfLevel = new List<int>();

	public XPEventsConfiguration Configuration
	{
		get;
		private set;
	}

	public XPEventsDatabase() : base(GTAssetTypes.configuration_file, "XPEventsConfiguration")
	{
		this.Configuration = null;
	}

    protected override void ProcessEventAssetDataString(ScriptableObject xpEventsConfiguration)
	{
	    this.Configuration = (XPEventsConfiguration) xpEventsConfiguration;
		this.CalculateLevelTables();
	}

	public int XPTotalAtEndOfLevel(int inLevel)
	{
		if (inLevel > 999)
		{
			inLevel = 999;
		}
		return this._xpTotalAtEndOfLevel[inLevel];
	}

	public int XPPerLevel(int inLevel)
	{
		if (inLevel > 999)
		{
			inLevel = 999;
		}
		return this._xpPerLevel[inLevel];
	}

	public void CalculateLevelTables()
	{
		this._xpPerLevel.Clear();
		this._xpTotalAtEndOfLevel.Clear();
		this._xpPerLevel.Add(0);
		this._xpTotalAtEndOfLevel.Add(0);
		int levelUpBaseXP = this.Configuration.LevelUpBaseXP;
		int levelUpIncrementPercent = this.Configuration.LevelUpIncrementPercent;
		int num = this._xpTotalAtEndOfLevel[0];
		for (int i = 1; i <= 999; i++)
		{
			int num2 = levelUpBaseXP;
			if (i > 0)
			{
				int num3 = levelUpBaseXP * (i - 1) * levelUpIncrementPercent / 100;
				num2 += num3;
			}
			this._xpPerLevel.Add(num2);
			num += num2;
			this._xpTotalAtEndOfLevel.Add(num);
#if UNITY_EDITOR
                GTDebug.Log(GTLogChannel.GameDatabase, "Level " + i + " Xp : " + num);
#endif
		}
	}

	public int GetPlayerLevel()
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (activeProfile == null)
		{
			return 0;
		}
		return activeProfile.GetPlayerLevel();
	}

	public int GetLevelForPlayerXP()
	{
		int playerXP = this.GetPlayerXP();
		return this.CurrentLevelForXP(playerXP);
	}

	public int GetPlayerXP()
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (activeProfile == null)
		{
			return 0;
		}
		return activeProfile.GetPlayerXP();
	}

	public void AddPlayerXP(int newXP)
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (activeProfile != null)
		{
			activeProfile.AddPlayerXP(newXP);
		}
	}

	public void LevelUpPlayerToXP(out int gold)
	{
		int playerLevel = this.GetPlayerLevel();
		int num = this.GetLevelForPlayerXP() - playerLevel;
		gold = this.CalculateGoldReward();
		
		PlayerProfileManager.Instance.ActiveProfile.AddGold(gold, "reward", "levelUp");
		for (int i = 0; i < num; i++)
		{
			PlayerProfileManager.Instance.ActiveProfile.IncrementPlayerLevel();
		}
		PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
	}

	public int GetLevelUpExtraCashReward()
	{
		VideoForRewardConfiguration configuration = GameDatabase.Instance.Ad.GetConfiguration(VideoForRewardConfiguration.eRewardID.VideoForExtraCashPrize);
		return VideoForRewardsManager.GetRewardAmount(configuration);
	}

	public void GiveLevelUpExtraCashReward()
	{
		int cash = GetLevelUpExtraCashReward();
		/*int cash = GameDatabase.Instance.Ad.GetLevelupRewardCash(PlayerProfileManager.Instance.ActiveProfile
			.GetHighestCarTierOwned());*/
		PlayerProfileManager.Instance.ActiveProfile.AddCash(cash, "reward", "levelUp");
		PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
	}

	public int CalculateGoldReward()
	{
        int playerLevel = this.GetPlayerLevel();
        int levelUpCount = this.GetLevelForPlayerXP() - playerLevel;
        float goldReward = 0f;
        float levelupBaseGold = (float)this.Configuration.LevelUpBaseGold;
        float incrementPercentGold = (float)this.Configuration.LevelUpIncrementPercentGold / 100f;
        for (int i = 0; i < levelUpCount; i++)
        {
            goldReward += levelupBaseGold * (1f + (float)(playerLevel + i) * incrementPercentGold);
        }
        return (int)goldReward;
        //return this.Configuration.LevelUpBaseGold;
    }

	public int CurrentLevelForXP(int XPAmount)
	{
		int i;
		for (i = 1; i < 999; i++)
		{
			if (XPAmount < this.XPTotalAtEndOfLevel(i))
			{
				return i;
			}
		}
		return i;
	}

	public float ProgressThroughLevelForXP(int XPAmount, int level)
	{
		int num = this.XPTotalAtEndOfLevel(level - 1);
		int num2 = this.XPPerLevel(level);
		int num3 = XPAmount - num;
		return Mathf.Clamp01((float)num3 / (float)num2);
	}

	public int GetXPPrizeForRaceComplete()
	{
		return this.GetXPPrizeForRaceComplete(true);
	}

	public int GetXPPrizeForRaceComplete(bool RaceWon)
	{
		var totalXp = 0;
		var currentEvent = RaceEventInfo.Instance.CurrentEvent;
		if (currentEvent != null && currentEvent.Parent != null)
		{
			totalXp = (int) currentEvent.RaceReward.XPPrize;
			if (totalXp == -1)
			{
				if (currentEvent.IsBossRace())
				{
					totalXp = this.Configuration.XPForCrewBoss;
				}
				else if (currentEvent.IsCrewRace())
				{
					totalXp = this.Configuration.XPForCrewMember;
				}
				else
				{
					totalXp = this.Configuration.XPForRaceEvent;
				}
			}
		}
        var tierXPRatio = (float)this.Configuration.XPTierModifier / 100f * (float)RaceEventInfo.Instance.CurrentEventTier;
        totalXp = (int)(totalXp + totalXp * tierXPRatio);
        if (!RaceWon)
        {
            var loseXp = (float)this.Configuration.XPLossModifier / 100f;
            totalXp = (int)(totalXp * loseXp);
        }
		return totalXp;
	}

	public int GetXPPrizeForPurchase()
	{
		return this.Configuration.XPForPurchase;
	}

    public int GetXPPrizeForPropertyPurchase()
    {
        return 0;//this.Configuration.XPForPurchase;
    }

	public int GetXPPrizeForUpgradePurchase(CarUpgradeData upgrade)
	{
		int currentTier = (int)PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().CurrentTier;
		int num = 0;
		float num2 = 0f;
		switch (upgrade.UpgradeType)
		{
		case eUpgradeType.ENGINE:
			num = this.Configuration.XPForEngineUpgrade;
			num2 = (float)this.Configuration.XPStageEngineModifier / 100f;
			break;
		case eUpgradeType.TURBO:
			num = this.Configuration.XPForTurboUpgrade;
			num2 = (float)this.Configuration.XPStageTurboModifier / 100f;
			break;
		case eUpgradeType.INTAKE:
			num = this.Configuration.XPForIntakeUpgrade;
			num2 = (float)this.Configuration.XPStageIntakeModifier / 100f;
			break;
		case eUpgradeType.NITROUS:
			num = this.Configuration.XPForNitrousUpgrade;
			num2 = (float)this.Configuration.XPStageNitrousModifier / 100f;
			break;
		case eUpgradeType.BODY:
			num = this.Configuration.XPForBodyUpgrade;
			num2 = (float)this.Configuration.XPStageBodyModifier / 100f;
			break;
		case eUpgradeType.TYRES:
			num = this.Configuration.XPForTyresUpgrade;
			num2 = (float)this.Configuration.XPStageTyresModifier / 100f;
			break;
		case eUpgradeType.TRANSMISSION:
			num = this.Configuration.XPForTransUpgrade;
			num2 = (float)this.Configuration.XPStageTransModifier / 100f;
			break;
		}
		float num3 = 1f + (float)currentTier * ((float)this.Configuration.XPTierModifier / 100f);
		float num4 = 1f + (float)upgrade.UpgradeLevel * num2;
		float num5 = num3 * num4;
		float num6 = (float)num * num5;
		return (int)num6;
	}

	public int GetXPPrizeForAchievement()
	{
		return this.Configuration.XPForAchievement;
	}

	public int GetXPPrizeForEvent()
	{
		return this.Configuration.XPForEvent;
	}
}
