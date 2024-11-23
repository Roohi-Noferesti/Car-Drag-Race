using System;
using System.Collections.Generic;
using System.Linq;

public static class RPBonusManager
{
	private static Dictionary<RP_BONUS_TYPE, Type> bonusTypes = new Dictionary<RP_BONUS_TYPE, Type>
	{
		{
			RP_BONUS_TYPE.RP_BONUS_EVENT,
			typeof(RPBonusCarSpecific)
		},
		{
			RP_BONUS_TYPE.CAR_BONUS,
			typeof(RPBonusCars)
		},
		{
			RP_BONUS_TYPE.CAR_SPECIFIC_BONUS,
			typeof(RPBonusCarSpecific)
		},
		{
			RP_BONUS_TYPE.CARD_BONUS,
			typeof(RPBonusCard)
		},
		{
			RP_BONUS_TYPE.STREAK_BONUS,
			typeof(RPBonusChain)
		},
		{
			RP_BONUS_TYPE.VIDEOAD_BONUS,
			typeof(RPBonusAd)
		},
		{
			RP_BONUS_TYPE.MULTIPLAYER_EVENT,
			typeof(RPBonusMultiplayerEvent)
		}
	};

	public static List<RPBonus> Bonuses = new List<RPBonus>();

	public static HashSet<RPBonus> UniqueBonuses = new HashSet<RPBonus>();

	public static RPBonusConfiguration RPBonusConfiguration
	{
		get
		{
			return GameDatabase.Instance.RPBonusConfiguration;
		}
	}

	public static void Initialise()
	{
		RPBonusManager.Bonuses.Clear();
		foreach (RPMultiplierBonus current in RPBonusManager.RPBonusConfiguration.RPBonuses)
		{
			if (current.Type != RP_BONUS_TYPE.INVALID)
			{
				Type type = RPBonusManager.bonusTypes[current.Type];
				RPBonus rPBonus = (RPBonus)Activator.CreateInstance(type);
				rPBonus.Populate(current);
				RPBonusManager.Bonuses.Add(rPBonus);
			}
		}
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		RPBonusManager.Bonuses.AddRange(from b in activeProfile.MultiplayerEventRPBonuses
		select b.GetBonus() into b
		where b != null
		select b);
	}

	public static float GetOverallMultiplier()
	{
		float multiplier = 1f;
		RPBonusManager.Bonuses.ForEach(delegate(RPBonus x)
		{
			if (x.AwardThisBonus())
			{
				multiplier += x.Multiplier;
			}
		});
		foreach (RPBonus current in RPBonusManager.UniqueBonuses)
		{
			if (current.AwardThisBonus())
			{
				multiplier += current.Multiplier;
			}
		}
		return multiplier;
	}

	public static float NavBarValue()
	{
		return (RPBonusManager.GetOverallMultiplier() - 1f) * 100f;
	}

	public static RPBonusAd GetRPBonusAd()
	{
		foreach (RPBonus current in RPBonusManager.UniqueBonuses)
		{
			if (current.GetBoostType() == RP_BONUS_TYPE.VIDEOAD_BONUS)
			{
				return current as RPBonusAd;
			}
		}
		return null;
	}

	public static RPReward GetRPCardReward(RewardSize rewardSize)
	{
		return RPBonusManager.RPBonusConfiguration.RPRewards.Find((RPReward x) => x.RewardSize == rewardSize);
	}

	public static void Unload()
	{
		RPBonusManager.Bonuses.Clear();
	}

	public static void AddBonus(RPBonus bonus)
	{
		RPBonusManager.Bonuses.Add(bonus);
		RPBonusManager.RefreshUI();
	}

	public static void AddUniqueBonusObject(RPBonus bonus)
	{
		if (!RPBonusManager.UniqueBonuses.Contains(bonus))
		{
			RPBonusManager.UniqueBonuses.Add(bonus);
			RPBonusManager.RefreshUI();
		}
	}

	public static void RemoveUniqueBonusObject(RPBonus bonus)
	{
		if (RPBonusManager.UniqueBonuses.Contains(bonus))
		{
			RPBonusManager.UniqueBonuses.Remove(bonus);
			RPBonusManager.RefreshUI();
		}
	}

	public static bool ContainsUniqueBonusObject(RPBonus bonus)
	{
		return RPBonusManager.UniqueBonuses.Contains(bonus);
	}

	public static void RefreshUI()
	{
        //if (CommonUI.Instance.RankBarMode == eRankBarMode.MULTIPLAYER_RANK)
        //{
        //    CommonUI.Instance.RPBonusStats.SetRPMultiplier(RPBonusManager.NavBarValue(), true);
        //}
	}
}
