using LitJson;
using System;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine;

public class CSR2ApplyableReward
{
	public ERewardType rewardType;

	public string name = string.Empty;

	public eUpgradeType partType = eUpgradeType.INVALID;

	public int partGrade;

	public int amount = 1;

	public ApplyableCarReward carReward;

	public CSR2ApplyableReward()
	{
		this.rewardType = ERewardType.Invalid;
	}

	public CSR2ApplyableReward(ERewardType _rewardType, int _amount)
	{
		this.rewardType = _rewardType;
		this.amount = _amount;
	}

	public CSR2ApplyableReward(CSR2Reward reward, int _amount)
	{
		if (reward != null)
		{
			this.rewardType = reward.rewardType;
			this.name = reward.name;
			this.partType = reward.ePartType;
			this.partGrade = reward.partGrade;
			this.amount = _amount;
		}
	}

	public CSR2ApplyableReward(CSR2ApplyableReward reward, int _amount)
	{
		if (reward != null)
		{
			this.rewardType = reward.rewardType;
			this.name = reward.name;
			this.partType = reward.partType;
			this.partGrade = reward.partGrade;
			this.amount = _amount;
		}
	}

	public CSR2ApplyableReward(RewardBase reward)
	{
		this.rewardType = reward.RewardType;
		this.amount = reward.GetRewardAmount();
        //switch (this.rewardType)
        //{
        //case ERewardType.Car:
        //{
        //    CarReward carReward = reward as CarReward;
        //    this.name = carReward.CarModel;
        //    break;
        //}
        //case ERewardType.EvoUpgrade:
        //{
        //    EvoUpgradeReward evoUpgradeReward = reward as EvoUpgradeReward;
        //    this.name = evoUpgradeReward.CarModel;
        //    this.partType = evoUpgradeReward.PartType;
        //    break;
        //}
        //case ERewardType.FusionUpgrade:
        //{
        //    FusionUpgradeReward fusionUpgradeReward = reward as FusionUpgradeReward;
        //    this.name = fusionUpgradeReward.Manufacturer;
        //    this.partType = fusionUpgradeReward.PartType;
        //    this.partGrade = fusionUpgradeReward.Grade;
        //    break;
        //}
        //}
	}

	public void SerialiseToJsonDict(out JsonDict dict)
	{
		dict = new JsonDict();
		dict.Set("rewardType", Convert.ToInt32(this.rewardType));
		dict.Set("name", this.name);
		dict.Set("partType", Convert.ToInt32(this.partType));
		dict.Set("partGrade", this.partGrade);
		dict.Set("amount", this.amount);
		if (this.carReward != null)
		{
			JsonDict value;
			this.carReward.SerialiseToJson(out value);
			dict.Set("carAward", value);
		}
	}

	public void SerializeFromJsonDict(JsonDict dict)
	{
		if (dict.ContainsKey("rewardType"))
		{
			this.rewardType = (ERewardType)dict.GetInt("rewardType");
		}
		if (dict.ContainsKey("name"))
		{
			this.name = dict.GetString("name");
		}
		if (dict.ContainsKey("partType"))
		{
			this.partType = (eUpgradeType)dict.GetInt("partType");
		}
		if (dict.ContainsKey("partGrade"))
		{
			this.partGrade = dict.GetInt("partGrade");
		}
		if (dict.ContainsKey("amount"))
		{
			this.amount = dict.GetInt("amount");
		}
		if (dict.ContainsKey("carAward"))
		{
			this.carReward = new ApplyableCarReward();
			this.carReward.ParseFromJSon(dict.GetJsonDict("carAward"));
		}
	}

	public bool CanApplyAwardToPlayerProfile()
	{
		if (PlayerProfileManager.Instance.ActiveProfile == null)
		{
			return false;
		}
		ERewardType eRewardType = this.rewardType;
		bool result;
		if (eRewardType != ERewardType.WildcardToken)
		{
			result = true;
		}
		else
		{
			result = true;
            //if (CrewWildcardManager.Instance.METADATA_maximumTokenCapacity > 0L && CrewWildcardManager.Instance.TokenBalance + (long)this.amount > CrewWildcardManager.Instance.METADATA_maximumTokenCapacity)
            //{
            //    result = false;
            //}
		}
		return result;
	}

	public bool ApplyAwardToPlayerProfile(bool fromGacha = false, bool immediateSave = true, bool extraReward = false)
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (activeProfile == null)
		{
			return false;
		}
		bool flag = false;
		switch (this.rewardType)
		{
		case ERewardType.Cash:
			activeProfile.AddCash(extraReward?this.amount*2:this.amount,"reward","ApplyableReward");//, ECashEarnedReason.RaceReward);
			flag = true;
			break;;
		case ERewardType.Gold:
			activeProfile.AddGold(extraReward?this.amount*2:this.amount,"reward","ApplyableReward");//, EGoldEarnedReason.RaceReward);
			flag = true;
			break;;
		case ERewardType.LockupKey:
            activeProfile.AddGachaKeys(this.amount, GachaType.Bronze, EGachaKeysEarnedReason.RaceReward);
			flag = true;
			break;;
		case ERewardType.SilverGachaKey:
            activeProfile.AddGachaKeys(this.amount, GachaType.Silver, EGachaKeysEarnedReason.RaceReward);
			flag = true;
			break;;
		case ERewardType.GoldGachaKey:
            activeProfile.AddGachaKeys(this.amount, GachaType.Gold, EGachaKeysEarnedReason.RaceReward);
			flag = true;
			break;;
		case ERewardType.FuelPip:
			if (FuelManager.Instance != null)
			{
			    FuelManager.Instance.AddFuel(extraReward?this.amount*2:this.amount, FuelReplenishTimeUpdateAction.UPDATE, FuelAnimationLockAction.OBEY);
			    //ZTrackMetricsHelper.LogCountRefillGas("consumable reward", this.amount);
			}
			flag = true;
			break;;
		case ERewardType.Car:
		case ERewardType.GachaConfigCar:
			if (!string.IsNullOrEmpty(this.name))
			{
				if (this.rewardType == ERewardType.GachaConfigCar)
				{
					fromGacha = true;
				}
				if (this.carReward == null)
				{
                    //int carUID = activeProfile.GiveCar(this.name, (!fromGacha) ? eCarSource.GenericRewards : eCarSource.Gacha, 0, -2147483648, -2147483648, null, false, -2147483648, -2147483648, null, -2147483648);
					if (fromGacha)
					{
                        //CarGarageInstance carFromUID = PlayerProfileManager.Instance.ActiveProfile.GetCarFromUID(carUID);
                        //ZTrackMetricsHelper.LogCarGacha(new MetricsTrackingID(), carFromUID);
                        //carFromUID.GachaConfigPending = true;
					}
					flag = true;
				}
				else
				{
					this.carReward.GiveCar(this.name);
					flag = true;
				}
			}
			break;;
		case ERewardType.EvoUpgrade:
			if (!string.IsNullOrEmpty(this.name))
			{
                //activeProfile.AddCarUpgradeEvoStage(new CarUpgradeEvoStage
                //{
                //    CarDBKey = this.name,
                //    UpgradeType = this.partType
                //});
				flag = true;
			}
			break;;
		case ERewardType.FusionUpgrade:
			if (!string.IsNullOrEmpty(this.name))
			{
				this.ApplyFusionUpgrade(activeProfile);
			}
			flag = true;
			break;;
		case ERewardType.TestRun:
            //activeProfile.FreeTestRunsLeft += this.amount;
			flag = true;
			break;;
		case ERewardType.Crate:
            //activeProfile.AddGachaFreeSpins(this.amount);
			flag = true;
			break;;
		case ERewardType.RP:
            //activeProfile.AddRP(this.amount, ERPEarnedReason.Reward);
			flag = true;
			break;;
		case ERewardType.FreeUpgrade:
			activeProfile.AddFreeUpgrade(extraReward?this.amount*2:this.amount);
			break;;
		case ERewardType.CrewRP:
		{
			//int eventID = 0;
			//int seriesID = 0;
			//string eventType = string.Empty;
            //if (RaceEventInfo.instance.CurrentEvent != null && RaceEventInfo.instance.CurrentEvent.Parent is SeriesEvent)
            //{
            //    eventID = RaceEventInfo.instance.CurrentEvent.EventID;
            //    seriesID = (RaceEventInfo.instance.CurrentEvent.Parent as SeriesEvent).SeriesID;
            //    string seriesName = (RaceEventInfo.instance.CurrentEvent.Parent as SeriesEvent).SeriesName;
            //    switch (seriesName)
            //    {
            //    case "TEXT_CUPS_TITLE_CREW":
            //        eventType = "crew_cup";
            //        goto IL_393;
            //    case "TEXT_CUPS_TITLE_PRESTIGE":
            //        eventType = "prestige_cup";
            //        goto IL_393;
            //    case "TEXT_CUPS_TITLE_TOKEN":
            //        eventType = "token_cup";
            //        goto IL_393;
            //    }
            //    eventType = string.Empty;
            //}
			//IL_393:
            //activeProfile.AddCrewRP(this.amount, ECrewRPEarnedReason.Reward, eventID, seriesID, eventType);
			flag = true;
			break;;
		}
		case ERewardType.WildcardToken:
            //activeProfile.AddWildcardToken(this.amount, this.rewardType, EWildcardEarnedReason.Reward);
			flag = true;
			break;;
		}
		if (flag)
		{
			if (immediateSave)
			{
				PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
			}
			else
			{
				PlayerProfileManager.Instance.RequestConvenientSaveActiveProfile();
			}
		}
		return flag;
	}

	public void FillAnyRandomPrizes(CSR2Reward rewardSpec)
	{
		this.name = rewardSpec.name;
		this.partGrade = rewardSpec.partGrade;
		this.partType = rewardSpec.ePartType;
		this.rewardType = rewardSpec.rewardType;
		switch (this.rewardType)
		{
		case ERewardType.Car:
			if (this.name == string.Empty)
			{
				CarInfo randomCar = CarDatabase.Instance.GetRandomCar();
				this.name = randomCar.Key;
			}
			break;
		case ERewardType.EvoUpgrade:
			if (this.name == string.Empty)
			{
				if (rewardSpec.matchOwnedCar)
				{
                    //PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
                    //List<CarGarageInstance> list = activeProfile.CarsOwned.FindAll((CarGarageInstance c) => c.CarTier > eCarTier.TIER_1 && !CarDatabase.Instance.IsBossCar(c.CarDBKey) && !CarDatabase.Instance.GetCar(c.CarDBKey).IsHeroCar);
                    //string text = string.Empty;
                    //if (list != null && list.Count > 0)
                    //{
                    //    CarGarageInstance carGarageInstance = list[UnityEngine.Random.Range(0, list.Count)];
                    //    text = carGarageInstance.CarDBKey;
                    //}
                    //else
                    //{
                    //    CarInfo randomCarEligibleForEvoUpgrade = CarDatabase.Instance.GetRandomCarEligibleForEvoUpgrade();
                    //    text = randomCarEligibleForEvoUpgrade.Key;
                    //}
                    //this.name = text;
				}
				else
				{
					//CarInfo carInfo;
                    //if (this.partGrade > 0)
                    //{
                    //    eCarTier zCarTier = (eCarTier)(this.partGrade - 1);
                    //    carInfo = CarDatabase.Instance.GetRandomCarOfTier(zCarTier);
                    //}
                    //else
                    //{
                    //    carInfo = CarDatabase.Instance.GetRandomCarEligibleForEvoUpgrade();
                    //}
                    //this.name = carInfo.Key;
				}
			}
			if (this.partType == eUpgradeType.INVALID)
			{
				this.partType = (eUpgradeType)UnityEngine.Random.Range(0, 7);
			}
			break;
		case ERewardType.FusionUpgrade:
			if (this.name == string.Empty)
			{
				if (rewardSpec.matchOwnedCar)
				{
					List<string> list2 = new List<string>();
					foreach (CarGarageInstance current in PlayerProfileManager.Instance.ActiveProfile.CarsOwned)
					{
						//CarInfo car = CarDatabase.Instance.GetCar(current.CarDBKey);
                        //if (!list2.Contains(car.ManufacturerID))
                        //{
                        //    list2.Add(car.ManufacturerID);
                        //}
					}
					this.name = list2[UnityEngine.Random.Range(0, list2.Count)];
				}
				else
				{
                    //Manufacturer randomManufacturer = ManufacturerDatabase.Instance.GetRandomManufacturer();
                    //if (randomManufacturer == null)
                    //{
                    //}
                    //this.name = randomManufacturer.id;
				}
			}
			if (this.partType == eUpgradeType.INVALID)
			{
				this.partType = (eUpgradeType)UnityEngine.Random.Range(0, 7);
			}
			if (this.partGrade == 0)
			{
				int max = 4;
                //if (!CarDatabase.Instance.HasManufacturerGotCarAboveTier(this.name, eCarTier.TIER_1))
                //{
                //    max = 3;
                //}
				this.partGrade = UnityEngine.Random.Range(1, max);
			}
			break;
		}
	}

	private void ApplyFusionUpgrade(PlayerProfile profile)
	{
        //FusionUpgradeReward.AwardFusionPartReward(this.name, this.partType, this.partGrade);
	}

	public override string ToString()
	{
		switch (this.rewardType)
		{
		case ERewardType.Car:
			return LocalizationManager.GetTranslation("TEXT_CAR_" + this.name + "_LONG");
		case ERewardType.EvoUpgrade:
			return string.Concat(new object[]
			{
				LocalizationManager.GetTranslation("TEXT_CAR_" + this.name + "_LONG"),
				" Stage 6 ",
				this.partType,
				" Evo Upgrade"
			});
		case ERewardType.FusionUpgrade:
			return string.Concat(new object[]
			{
				LocalizationManager.GetTranslation("TEXT_MANUFACTURERS_" + this.name),
				" Grade ",
				this.partGrade,
				" ",
				this.partType,
				" Fusion Upgrade"
			});
		default:
			return this.amount + " " + this.rewardType;
		}
	}

	public string GetRewardString(int amount)
	{
		string text = string.Empty;
		switch (this.rewardType)
		{
		case ERewardType.Cash:
			text += "a cash bonus";
			return text;
		case ERewardType.Gold:
			text += "a gold bonus";
			return text;
		case ERewardType.TireSlicks:
			text += "some bonus tire slicks";
			return text;
		case ERewardType.SpecialNitrous:
			text += "bonus special nitrous";
			return text;
		case ERewardType.GearService:
			text += "a gear service bonus";
			return text;
		case ERewardType.RacingFuel:
			text += "a racing fuel bonus";
			return text;
		case ERewardType.LockupKey:
			text = text + amount + " bronze gacha keys";
			return text;
		case ERewardType.SilverGachaKey:
			text = text + amount + " silver gacha keys";
			return text;
		case ERewardType.GoldGachaKey:
			text = text + amount + " gold gacha keys";
			return text;
		case ERewardType.FuelPip:
			text = text + amount + " bonus fuel pips";
			return text;
		case ERewardType.Car:
			text = text + "a " + this.name + " as a bonus";
			return text;
		case ERewardType.EvoUpgrade:
			text += string.Format("a bonus {0} stage 6\n{1} upgrade part", this.name, this.partType);
			return text;
		case ERewardType.FusionUpgrade:
			text += string.Format("a bonus {0} {1}\nstage {2} fusion part", this.name, this.partType, this.partGrade);
			return text;
		case ERewardType.Crate:
			text += string.Format("{0} gacha creates awarded", amount);
			return text;
		case ERewardType.RP:
			text += "TEXT_PRIZE_RP_DESC";
			return text;
		case ERewardType.FreeUpgrade:
			text += "a free upgrade";
			return text;
		case ERewardType.CrewRP:
			text += "TEXT_PRIZE_CREWRP_DESC";
			return text;
		}
		text += "an unknown (?!) reward";
		return text;
	}


    public string GetRewardText()
    {
        switch (rewardType)
        {
            case ERewardType.Cash:
                return CurrencyUtils.GetCashString(amount);
            case ERewardType.Gold:
                return CurrencyUtils.GetGoldStringWithIcon(amount);
            case ERewardType.FreeUpgrade:
                return LocalizationManager.GetTranslation("TEXT_FREE_UPGRADE");
            case ERewardType.FuelPip:
                return string.Format(LocalizationManager.GetTranslation("TEXT_FILL_FUEL_PIPS"), amount);
            //case ERewardType.Car:
            //    return Reward.GetRewardTitleText(amount);
        }
        return string.Empty;
    }
}
