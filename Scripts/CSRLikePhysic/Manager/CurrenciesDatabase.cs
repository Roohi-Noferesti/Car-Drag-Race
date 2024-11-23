using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Z2HSharedLibrary.DatabaseEntity;

public class CurrenciesDatabase : ConfigurationAssetLoader
{
	private const int DEFAULT_GOLD_TO_FILL_TANK = 5;

	public CurrenciesConfiguration Configuration
	{
		get;
		private set;
	}

	public CurrenciesDatabase() : base(GTAssetTypes.configuration_file, "CurrenciesConfiguration")
	{
		this.Configuration = null;
	}

    protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
	{
	    this.Configuration = (CurrenciesConfiguration) scriptableObject;//JsonConverter.DeserializeObject<CurrenciesConfiguration>(assetDataString);
	}

	public void getLiveryBonusAmounts(eCarTier carTier, string itemId, out int baseLiveryBonus, out float liveryGoldMultiplier, out float liveryTierMultiplier, out float customizationTypeMultiplier)
	{
		baseLiveryBonus = this.Configuration.RewardsMultipliers.BaseLiveryWinBonus;
		liveryGoldMultiplier = this.Configuration.RewardsMultipliers.LiveryWinGoldMultiplier;
		GetCustomizationMultiplier(itemId, out customizationTypeMultiplier);
		switch (carTier)
		{
		case eCarTier.TIER_1:
			liveryTierMultiplier = this.Configuration.RewardsMultipliers.LiveryWinTier1Multiplier;
			break;
		case eCarTier.TIER_2:
			liveryTierMultiplier = this.Configuration.RewardsMultipliers.LiveryWinTier2Multiplier;
			break;
		case eCarTier.TIER_3:
			liveryTierMultiplier = this.Configuration.RewardsMultipliers.LiveryWinTier3Multiplier;
			break;
		case eCarTier.TIER_4:
			liveryTierMultiplier = this.Configuration.RewardsMultipliers.LiveryWinTier4Multiplier;
			break;
		case eCarTier.TIER_5:
			liveryTierMultiplier = this.Configuration.RewardsMultipliers.LiveryWinTier5Multiplier;
			break;
		case eCarTier.TIER_X:
			liveryTierMultiplier = this.Configuration.RewardsMultipliers.LiveryWinTier5Multiplier;
			break;
		default:
			liveryTierMultiplier = this.Configuration.RewardsMultipliers.LiveryWinTier1Multiplier;
			break;
		}
	}

	private void GetCustomizationMultiplier(string liveryItem, out float customizationMultiplier)
	{
		
		string[] res = liveryItem.Split(new string[] { "_" }, StringSplitOptions.None);
		switch (res[0])
		{
			case "CarBody":
				customizationMultiplier = 0.2f;
				break;
			case "Ring":
				customizationMultiplier = 0.1f;
				break;
			case "HeadLight":
				customizationMultiplier = 0.1f;
				break;
			case "Sticker":
				customizationMultiplier = 0.3f;
				break;
			case "Spoiler":
				customizationMultiplier = 0.3f;
				break;
			default:
				customizationMultiplier = 0.1f;
				break;
		}
	}

	public int GetChangeNumberPlateCost()
	{
		return 2;
	}

	public RaceEventTypeMultipliers getRaceEventTypeRewardMultipliers(RaceEventData raceEvent)
	{
		return raceEvent.GetRewardsMultipliers(this.Configuration.RewardsMultipliers);
	}

	public DailyBattleMultipliers getDailyBattleRewardMultipliers()
	{
		return this.Configuration.RewardsMultipliers.DailyBattleMultipliers;
	}

	public List<MechanicData> getCurrentMechanicScreenData(eCarTier carTier)
	{
		List<MechanicData> list = new List<MechanicData>();

	    if (carTier == eCarTier.TIER_5)
		{
			list.Add(this.Configuration.Gold.Mechanics.Tier5Mechanic1);
			list.Add(this.Configuration.Gold.Mechanics.Tier5Mechanic2);
			list.Add(this.Configuration.Gold.Mechanics.Tier5Mechanic3);
		}
		else if (carTier == eCarTier.TIER_4)
		{
			list.Add(this.Configuration.Gold.Mechanics.Tier4Mechanic1);
			list.Add(this.Configuration.Gold.Mechanics.Tier4Mechanic2);
			list.Add(this.Configuration.Gold.Mechanics.Tier4Mechanic3);
		}
		else if (carTier == eCarTier.TIER_3)
		{
			list.Add(this.Configuration.Gold.Mechanics.Tier3Mechanic1);
			list.Add(this.Configuration.Gold.Mechanics.Tier3Mechanic2);
			list.Add(this.Configuration.Gold.Mechanics.Tier3Mechanic3);
		}
		else if (carTier == eCarTier.TIER_2)
		{
			list.Add(this.Configuration.Gold.Mechanics.Tier2Mechanic1);
			list.Add(this.Configuration.Gold.Mechanics.Tier2Mechanic2);
			list.Add(this.Configuration.Gold.Mechanics.Tier2Mechanic3);
		}
		list.Add(this.Configuration.Gold.Mechanics.Tier1Mechanic1);
		list.Add(this.Configuration.Gold.Mechanics.Tier1Mechanic2);
		list.Add(this.Configuration.Gold.Mechanics.Tier1Mechanic3);
		return list;
	}

	public int getCarDeliveryTime(eCarTier itsclass)
	{
		if (itsclass == eCarTier.TIER_5)
		{
			return this.Configuration.Deliveries.Tier5CarDeliveryTime;
		}
		if (itsclass == eCarTier.TIER_4)
		{
			return this.Configuration.Deliveries.Tier4CarDeliveryTime;
		}
		if (itsclass == eCarTier.TIER_3)
		{
			return this.Configuration.Deliveries.Tier3CarDeliveryTime;
		}
		if (itsclass == eCarTier.TIER_2)
		{
			return this.Configuration.Deliveries.Tier2CarDeliveryTime;
		}
		return this.Configuration.Deliveries.Tier1CarDeliveryTime;
	}

	public int GetPartDeliveryTime(eCarTier itsclass)
	{
		if (itsclass == eCarTier.TIER_5)
		{
			return this.Configuration.Deliveries.Tier5PartDeliveryTime;
		}
		if (itsclass == eCarTier.TIER_4)
		{
			return this.Configuration.Deliveries.Tier4PartDeliveryTime;
		}
		if (itsclass == eCarTier.TIER_3)
		{
			return this.Configuration.Deliveries.Tier3PartDeliveryTime;
		}
		if (itsclass == eCarTier.TIER_2)
		{
			return this.Configuration.Deliveries.Tier2PartDeliveryTime;
		}
		return this.Configuration.Deliveries.Tier1PartDeliveryTime;
	}

	public int getSecondsGainedPerGold()
	{
		return this.Configuration.Deliveries.SpeedUpSecondsPerGold;
	}

	public int GetFuelCostForEvent(RaceEventData raceEvent)
	{
		if (!raceEvent.RaceCostsFuel())
		{
			return 0;
		}
		if (raceEvent.IsBossRace())
		{
			return this.Configuration.Fuel.BossBattleCost;
		}
		if (raceEvent.IsCrewBattle())
		{
			return this.Configuration.Fuel.CrewBattleCost;
		}
		if (raceEvent.IsLadderEvent())
		{
			return this.Configuration.Fuel.LadderRaceCost;
		}
		if (raceEvent.IsDailyBattle())
		{
			return this.Configuration.Fuel.DailyBattleCost;
		}
		if (raceEvent.IsCarSpecificEvent())
		{
			return this.Configuration.Fuel.CarSpecificCost;
		}
		if (raceEvent.IsRegulationRace())
		{
			return this.Configuration.Fuel.RegulationRaceCost;
		}
		if (raceEvent.IsRestrictionEvent())
		{
			return this.Configuration.Fuel.RestrictionCost;
		}
		if (raceEvent.IsManufacturerSpecificEvent())
		{
			return this.Configuration.Fuel.ManufacturerCost;
		}
        if (raceEvent.IsSMPRaceEvent())
        {
            return this.Configuration.Fuel.SMPCost;
        }
		if (!raceEvent.IsRelay)
		{
			return this.Configuration.Fuel.DefaultOtherCost;
		}
		if (raceEvent.IsGrindRelay())
		{
			return RelayManager.GetRaceCount(raceEvent) - 2;
		}
		if (raceEvent.IsRandomRelay())
		{
			return RelayManager.GetRaceCount(raceEvent);
		}
		if (raceEvent.GetRelayRaceIndex() == 0)
		{
			return this.Configuration.Fuel.RelayRaceLegCost * RelayManager.GetRaceCount(raceEvent);
		}
		return 0;
	}

	public int GetBaseRewardForRaceTheWorld(eCarTier tier)
	{
		if (tier == eCarTier.TIER_2)
		{
			return this.Configuration.RewardsMultipliers.RaceTheWorldMultipliers.BaseRewardT2;
		}
		if (tier == eCarTier.TIER_3)
		{
			return this.Configuration.RewardsMultipliers.RaceTheWorldMultipliers.BaseRewardT3;
		}
		if (tier == eCarTier.TIER_4)
		{
			return this.Configuration.RewardsMultipliers.RaceTheWorldMultipliers.BaseRewardT4;
		}
		if (tier == eCarTier.TIER_5)
		{
			return this.Configuration.RewardsMultipliers.RaceTheWorldMultipliers.BaseRewardT5;
		}
		return this.Configuration.RewardsMultipliers.RaceTheWorldMultipliers.BaseRewardT1;
	}

	public RaceTheWorldMultipliers GetRaceTheWorldMultipliers()
	{
		return this.Configuration.RewardsMultipliers.RaceTheWorldMultipliers;
	}

	public ConsumableTypeData GetConsumableData(eCarConsumables type)
	{
		if (type == eCarConsumables.EngineTune)
		{
			return this.Configuration.Consumables.EngineConsumables;
		}
		if (type == eCarConsumables.Nitrous)
		{
			return this.Configuration.Consumables.NitrousConsumables;
		}
		if (type == eCarConsumables.PRAgent)
		{
			return this.Configuration.Consumables.PRAgentConsumables;
		}
		return this.Configuration.Consumables.TyreConsumables;
	}

	public int CalculateLiveryRaceBonus(eCarTier tier, int LiveryGoldCost)
	{
		int num = 0;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		string itemId = null;
		this.getLiveryBonusAmounts(tier,itemId, out num, out num2, out num3, out num4);
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
		DateTime value = dateTime.AddSeconds((double)this.Configuration.RewardsMultipliers.LiveryBonusTypeDateBoundary);
		bool flag = PlayerProfileManager.Instance.ActiveProfile.UserStartedPlaying.CompareTo(value) <= 0;
		if (flag)
		{
			return Mathf.CeilToInt((float)(num + 15 * LiveryGoldCost) * num3);
		}
		int num5 = 0;
		if (LiveryGoldCost > 0)
		{
			num5 = Mathf.CeilToInt(Mathf.Log((float)LiveryGoldCost, 2.71828f));
		}
		return Mathf.CeilToInt(((float)num + num2 * (float)num5) * num3 * num4);
	}

	public int GetGoldToFillFuelTank()
	{
		return (this.Configuration.Fuel == null) ? 5 : this.Configuration.Fuel.GoldToFillTank;
	}

    public int GetcashRewardByPPIndexAndDifficulty(int ppindex, AutoDifficulty.DifficultyRating rating, eCarTier carTier,
        eCarTier baseTier)
    {
        //if (carTier != baseTier)
        //{
        //    ppindex = CarDatabase.Instance.GetCarsOfTier(baseTier).OrderBy(c => c.PPIndex).First().PPIndex;
        //}

        //var rewardPower = Configuration.RewardsMultipliers.RaceRewardPower;
        //var basereward = Mathf.Pow(ppindex, rewardPower);
        //switch (baseTier)
        //{
        //    case eCarTier.TIER_1:
        //        basereward *= 1.2F;
        //        break;
        //    case eCarTier.TIER_2:
        //        basereward *= 1;
        //        break;
        //    case eCarTier.TIER_3:
        //        basereward *= 1.36F;
        //        break;
        //    case eCarTier.TIER_4:
        //        basereward *= 2;
        //        break;
        //    case eCarTier.TIER_5:
        //        basereward *= 3;
        //        break;
        //    case eCarTier.TIER_6:
        //        basereward *= 3;
        //        break;
        //}
        //switch (rating)
        //{
        //    case AutoDifficulty.DifficultyRating.Easy:
        //        return (int) (basereward*1).RoundTo(50);
        //    case AutoDifficulty.DifficultyRating.Challenging:
        //        return (int) (basereward*1.3F).RoundTo(50);
        //    case AutoDifficulty.DifficultyRating.Difficult:
        //        return (int) (basereward*1.6F).RoundTo(50);
        //}

        RegulationRewards.RegulationDifficultyReward reward = null;
        switch (baseTier)
        {
            case eCarTier.TIER_1:
                reward = Configuration.RewardsMultipliers.RegulationRaceMultipliers.Tier1;
                break;
            case eCarTier.TIER_2:
                reward = Configuration.RewardsMultipliers.RegulationRaceMultipliers.Tier2;
                break;
            case eCarTier.TIER_3:
                reward = Configuration.RewardsMultipliers.RegulationRaceMultipliers.Tier3;
                break;
            case eCarTier.TIER_4:
                reward = Configuration.RewardsMultipliers.RegulationRaceMultipliers.Tier4;
                break;
            case eCarTier.TIER_5:
                reward = Configuration.RewardsMultipliers.RegulationRaceMultipliers.Tier5;
                break;
            case eCarTier.TIER_X:
                reward = Configuration.RewardsMultipliers.RegulationRaceMultipliers.TierX;
                break;
        }

        switch (rating)
        {
            case AutoDifficulty.DifficultyRating.Easy:
                return reward.EasyReward;
            case AutoDifficulty.DifficultyRating.Challenging:
                return reward.NormalReward;
            case AutoDifficulty.DifficultyRating.Difficult:
                return reward.HardReward;
        }

        return 0;
    }
}
