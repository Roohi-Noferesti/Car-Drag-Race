using System;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using UnityEngine;

[Serializable]
public class RaceEventRestriction
{
	public enum RestrictionMet
	{
		TRUE,
		FALSE,
		UNKNOWN
	}

	public eRaceEventRestrictionType RestrictionType;

	public string Manufacturer = "None";

    public string Classes;

	public short Weight;

	public short Horsepower;

	public int Cash;

	public string Region;

	public string RestrictionTypeString;

	public string Model;

	public eDriveType DriveWheels;

	public List<string> GetCarModels()
	{
		if (string.IsNullOrEmpty(this.Model))
		{
			return new List<string>();
		}
		return new List<string>(this.Model.Split(new char[]
		{
			'|'
		}));
	}

	public void Initialise()
	{
		this.RestrictionType = EnumHelper.FromString<eRaceEventRestrictionType>(this.RestrictionTypeString);
	}

	public void SetCarModels(List<string> CarList)
	{
		this.Model = string.Join("|", CarList.ToArray());
	}

	public void SetCarModel(string CarModel)
	{
		this.Model = CarModel;
	}

	public override string ToString()
	{
		string text = "Enum value : " + this.RestrictionType.ToString() + "\n";
		if (this.RestrictionType == eRaceEventRestrictionType.CAR_MANUFACTURER)
		{
			text = text + "Manufacturer : " + this.Manufacturer + "\n";
		}
		if (this.RestrictionType == eRaceEventRestrictionType.CAR_WEIGHT_LESS_THAN)
		{
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"Maximum Weight : ",
				this.Weight,
				"\n"
			});
		}
		if (this.RestrictionType == eRaceEventRestrictionType.CAR_WEIGHT_MORE_THAN)
		{
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"Minimum Weight : ",
				this.Weight,
				"\n"
			});
		}
		if (this.RestrictionType == eRaceEventRestrictionType.CAR_HORSEPOWER_LESS_THAN)
		{
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"Maximum Horsepower : ",
				this.Horsepower,
				"\n"
			});
		}
		if (this.RestrictionType == eRaceEventRestrictionType.CAR_HORSEPOWER_MORE_THAN)
		{
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"Minimum Horsepower : ",
				this.Horsepower,
				"\n"
			});
		}
		return text;
	}

	public string ToString(CarPhysicsSetupCreator zCarPhysicsData, string colourString = "")
	{
		string text = string.Empty;
		if (this.RestrictionType == eRaceEventRestrictionType.CAR_MANUFACTURER)
		{
			string format = LocalizationManager.GetTranslation("TEXT_RACE_EVENT_RESTRICTION_TYPE_MANUFACTURER");
			text = string.Format(format, colourString, this.Manufacturer);
		}
		if (this.RestrictionType == eRaceEventRestrictionType.CAR_WEIGHT_LESS_THAN)
		{
			string format2 = LocalizationManager.GetTranslation("TEXT_RACE_EVENT_RESTRICTION_TYPE_MAX_WEIGHT");
			text = string.Format(format2, colourString, this.Weight);
		}
		if (this.RestrictionType == eRaceEventRestrictionType.CAR_WEIGHT_MORE_THAN)
		{
			string format3 = LocalizationManager.GetTranslation("TEXT_RACE_EVENT_RESTRICTION_TYPE_MIN_WEIGHT");
			text = string.Format(format3, colourString, this.Weight);
		}
		if (this.RestrictionType == eRaceEventRestrictionType.CAR_HORSEPOWER_LESS_THAN)
		{
			string format4 = LocalizationManager.GetTranslation("TEXT_RACE_EVENT_RESTRICTION_TYPE_MAX_HORSEPOWER");
			text = string.Format(format4, colourString, this.Horsepower);
		}
		if (this.RestrictionType == eRaceEventRestrictionType.CAR_HORSEPOWER_MORE_THAN)
		{
			string format5 = LocalizationManager.GetTranslation("TEXT_RACE_EVENT_RESTRICTION_TYPE_MIN_HORSEPOWER");
			text = string.Format(format5, colourString, this.Horsepower);
		}
		if (this.RestrictionType == eRaceEventRestrictionType.CAR_DRIVE_WHEELS)
		{
			string format6 = LocalizationManager.GetTranslation("TEXT_RACE_EVENT_RESTRICTION_TYPE_DRIVE_WHEELS");
			text = string.Format(format6, colourString, LocalizationManager.GetTranslation(CarInfo.DriveWheelsString[(int)this.DriveWheels]));
		}
		if (this.RestrictionType == eRaceEventRestrictionType.CAR_MODEL)
		{
			string format7 = LocalizationManager.GetTranslation("TEXT_RACE_EVENT_RESTRICTION_TYPE_RESTRICTION");
			IEnumerable<string> source = from x in this.GetCarModels()
			select CarDatabase.Instance.GetCarNiceName(x);
			text = string.Format(format7, colourString, string.Join(", ", source.ToArray<string>()));
		}
		if (this.RestrictionType == eRaceEventRestrictionType.CAR_NO_UPGRADES_ALLOWED)
		{
			string format8 = LocalizationManager.GetTranslation("TEXT_RACE_EVENT_RESTRICTION_TYPE_NO_CAR_UPGRADE_ALLOWED");
			text = string.Format(format8, colourString);
		}
		if (this.RestrictionType == eRaceEventRestrictionType.CAR_NO_NITROUS_ALLOWED)
		{
			string format9 = LocalizationManager.GetTranslation("TEXT_RACE_EVENT_RESTRICTION_TYPE_NO_NITROUS_ALLOWED");
			text = string.Format(format9, colourString);
		}
		if (this.RestrictionType == eRaceEventRestrictionType.CAR_NO_TURBO_ALLOWED)
		{
			string format10 = LocalizationManager.GetTranslation("TEXT_RACE_EVENT_RESTRICTION_TYPE_NO_TURBO_ALLOWED");
			text = string.Format(format10, colourString);
		}
		if (this.RestrictionType == eRaceEventRestrictionType.CAR_NO_TYRES_ALLOWED)
		{
			string format11 = LocalizationManager.GetTranslation("TEXT_RACE_EVENT_RESTRICTION_TYPE_NO_TIRE_ALLOWED");
			text = string.Format(format11, colourString);
		}
		if (this.RestrictionType == eRaceEventRestrictionType.CAR_NITROUS_NEEDED)
		{
			string format12 = LocalizationManager.GetTranslation("TEXT_RACE_EVENT_RESTRICTION_TYPE_NITROUS_REQUIRED");
			text = string.Format(format12, colourString);
		}
		if (this.RestrictionType == eRaceEventRestrictionType.CAR_TYRES_NEEDED)
		{
			string format13 = LocalizationManager.GetTranslation("TEXT_RACE_EVENT_RESTRICTION_TYPE_TIRE_REQUIRED");
			text = string.Format(format13, colourString);
		}
		if (this.RestrictionType == eRaceEventRestrictionType.CASH_MORE_THAN)
		{
			string format14 = LocalizationManager.GetTranslation("TEXT_RACE_EVENT_RESTRICTION_TYPE_CASH_MORE_THAN");
			text = string.Format(format14, colourString);
		}
		if (this.RestrictionType == eRaceEventRestrictionType.CASH_LESS_THAN)
		{
			string format15 = LocalizationManager.GetTranslation("TEXT_RACE_EVENT_RESTRICTION_TYPE_CASH_LESS_THAN");
			text = string.Format(format15, colourString);
		}
		if (this.RestrictionType == eRaceEventRestrictionType.CAR_CLASS)
		{
			string arg = LocalizationManager.GetTranslation("TEXT_" + this.Classes);
			string format16 = string.Format(LocalizationManager.GetTranslation("TEXT_INCORRECT_TIER_BODY"), arg);
			text = string.Format(format16, colourString);
		}
		if (this.RestrictionType == eRaceEventRestrictionType.CAR_PP_MORE_THAN)
		{
			string format17 = LocalizationManager.GetTranslation("TEXT_RACE_EVENT_RESTRICTION_TYPE_PP_MORE_THAN");
			text = string.Format(format17, colourString, this.Horsepower);
		}
		if (this.RestrictionType == eRaceEventRestrictionType.CAR_PP_LESS_THAN)
		{
			string format18 = LocalizationManager.GetTranslation("TEXT_RACE_EVENT_RESTRICTION_TYPE_PP_LESS_THAN");
			text = string.Format(format18, colourString, this.Horsepower);
		}
		if (this.RestrictionType == eRaceEventRestrictionType.CAR_PRO)
		{
			string format19 = LocalizationManager.GetTranslation("TEXT_RACE_EVENT_RESTRICTION_TYPE_PRO");
			text = string.Format(format19, colourString);
		}
		if (text == string.Empty)
		{
			text += "Unknown restriction present.";
		}
		return text;
	}

	public string ToStringWithColour(CarPhysicsSetupCreator zCarPhysicsData)
	{
		string colourString = string.Empty + this.GetColourBasedOnRestriction(zCarPhysicsData);
		return this.ToString(zCarPhysicsData, colourString);
	}

	private Color GetColourBasedOnRestriction(CarPhysicsSetupCreator zCarPhysicsData)
	{
		return Color.white;
	}

	public RestrictionMet CanMeetWeightRestrictionNaive(CarInfo car)
	{
		float num;
		float num2;
		car.CalulateCosmeticMassRange(out num, out num2);
		num *= 2.20462251f;
		num2 *= 2.20462251f;
		if (this.RestrictionType == eRaceEventRestrictionType.CAR_WEIGHT_MORE_THAN)
		{
			if ((float)this.Weight >= num2)
			{
				return RestrictionMet.FALSE;
			}
			if (num > (float)this.Weight)
			{
				return RestrictionMet.TRUE;
			}
		}
		if (this.RestrictionType == eRaceEventRestrictionType.CAR_WEIGHT_LESS_THAN)
		{
			if ((float)this.Weight <= num)
			{
				return RestrictionMet.FALSE;
			}
			if (num2 < (float)this.Weight)
			{
				return RestrictionMet.TRUE;
			}
		}
		return RestrictionMet.UNKNOWN;
	}

	public RestrictionMet CanMeetCashRestrictionNaive()
	{
		int currentCash = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCash();
		if (this.RestrictionType == eRaceEventRestrictionType.CASH_MORE_THAN)
		{
			return (currentCash < this.Cash) ? RestrictionMet.FALSE : RestrictionMet.TRUE;
		}
		if (this.RestrictionType == eRaceEventRestrictionType.CASH_LESS_THAN)
		{
			return (currentCash > this.Cash) ? RestrictionMet.FALSE : RestrictionMet.TRUE;
		}
		return RestrictionMet.UNKNOWN;
	}

	public RestrictionMet CanMeetHPRestrictionNaive(CarInfo car)
	{
		float num = (float)car.FlyWheelPower;
		if (this.RestrictionType == eRaceEventRestrictionType.CAR_HORSEPOWER_MORE_THAN && num >= (float)this.Horsepower)
		{
			return RestrictionMet.TRUE;
		}
		if (this.RestrictionType == eRaceEventRestrictionType.CAR_HORSEPOWER_LESS_THAN && num > (float)this.Horsepower)
		{
			return RestrictionMet.FALSE;
		}
		return RestrictionMet.UNKNOWN;
	}

	public RestrictionMet CanMeetPPRestrictionNaive(CarInfo car)
	{
		float num = (float)car.BasePerformanceIndex;
		if (this.RestrictionType == eRaceEventRestrictionType.CAR_PP_MORE_THAN && num > (float)this.Horsepower)
		{
			return RestrictionMet.TRUE;
		}
		if (this.RestrictionType == eRaceEventRestrictionType.CAR_PP_LESS_THAN && num >= (float)this.Horsepower)
		{
			return RestrictionMet.FALSE;
		}
		return RestrictionMet.UNKNOWN;
	}

	public RestrictionMet CanMeetProRestrictionNaive(CarInfo car)
	{
		return (!PlayerProfileManager.Instance.ActiveProfile.IsProCarOwned(car.Key)) ? RestrictionMet.FALSE : RestrictionMet.TRUE;
	}

	public RestrictionMet DoesMeetRestrictionNaive(CarInfo car)
	{
		List<string> carModels = this.GetCarModels();
		List<string> list = (from m in carModels
		select m + "Boss").ToList<string>();
		switch (this.RestrictionType)
		{
		case eRaceEventRestrictionType.CAR_MANUFACTURER:
			if (car.ManufacturerName != this.Manufacturer)
			{
				return RestrictionMet.FALSE;
			}

			return RestrictionMet.TRUE;
		case eRaceEventRestrictionType.CAR_CLASS:
			if (CarInfo.ConvertCarTierEnumToString(car.BaseCarTier) != "TEXT_" + this.Classes)
			{
				return RestrictionMet.FALSE;
			}
			return RestrictionMet.TRUE;
		case eRaceEventRestrictionType.CAR_WEIGHT_MORE_THAN:
			return this.CanMeetWeightRestrictionNaive(car);
		case eRaceEventRestrictionType.CAR_WEIGHT_LESS_THAN:
			return this.CanMeetWeightRestrictionNaive(car);
		case eRaceEventRestrictionType.CAR_HORSEPOWER_MORE_THAN:
			return this.CanMeetHPRestrictionNaive(car);
		case eRaceEventRestrictionType.CAR_HORSEPOWER_LESS_THAN:
			return this.CanMeetHPRestrictionNaive(car);
		case eRaceEventRestrictionType.CAR_DRIVE_WHEELS:
			if (car.DriveType != this.DriveWheels)
			{
				return RestrictionMet.FALSE;
			}
			return RestrictionMet.TRUE;
		case eRaceEventRestrictionType.CAR_MODEL:
		    if (car.Key == "car_lamborghini_aventador")
		    {
		        int i = 0;
		    }
			if (!carModels.Contains(car.Key) && !list.Contains(car.Key))
			{
				return RestrictionMet.FALSE;
			}
			return RestrictionMet.TRUE;
		case eRaceEventRestrictionType.CASH_MORE_THAN:
		case eRaceEventRestrictionType.CASH_LESS_THAN:
			return this.CanMeetCashRestrictionNaive();
		case eRaceEventRestrictionType.CAR_PP_MORE_THAN:
		case eRaceEventRestrictionType.CAR_PP_LESS_THAN:
			return this.CanMeetPPRestrictionNaive(car);
		case eRaceEventRestrictionType.CAR_PRO:
			return this.CanMeetProRestrictionNaive(car);
		}
		return RestrictionMet.UNKNOWN;
	}

    public bool DoesMeetRestriction(CarPhysicsSetupCreator zCarPhysicsData)
    {
        int num = Mathf.CeilToInt(zCarPhysicsData.NewWeight * 2.20462251f);
        int newPeakHP = zCarPhysicsData.NewPeakHP;
        int newPerformanceIndex = zCarPhysicsData.NewPerformanceIndex;
        List<string> carModels = this.GetCarModels();
        List<string> list = (from m in carModels
            select m + "Boss").ToList<string>();
        switch (this.RestrictionType)
        {
            case eRaceEventRestrictionType.CAR_MANUFACTURER:
                if (zCarPhysicsData.carInfo.ManufacturerName != this.Manufacturer)
                {
                    return false;
                }

                return true;
            case eRaceEventRestrictionType.CAR_CLASS:
                if (CarInfo.ConvertCarTierEnumToString(zCarPhysicsData.BaseCarTier) != "TEXT_" + Classes)
                {
                    return false;
                }

                break;
            case eRaceEventRestrictionType.CAR_WEIGHT_MORE_THAN:
                if (num < (int) this.Weight)
                {
                    return false;
                }

                return true;
            case eRaceEventRestrictionType.CAR_WEIGHT_LESS_THAN:
                if (num > (int) this.Weight)
                {
                    return false;
                }

                return true;
            case eRaceEventRestrictionType.CAR_HORSEPOWER_MORE_THAN:
                if (newPeakHP < (int) this.Horsepower)
                {
                    return false;
                }

                return true;
            case eRaceEventRestrictionType.CAR_HORSEPOWER_LESS_THAN:
                if (newPeakHP > (int) this.Horsepower)
                {
                    return false;
                }

                return true;
            case eRaceEventRestrictionType.CAR_DRIVE_WHEELS:
                if (zCarPhysicsData.NewDriveType != this.DriveWheels)
                {
                    return false;
                }

                return true;
            case eRaceEventRestrictionType.CAR_MODEL:
                if (!carModels.Contains(zCarPhysicsData.CarModel) && !list.Contains(zCarPhysicsData.CarModel))
                {
                    return false;
                }

                return true;
            case eRaceEventRestrictionType.CAR_NO_UPGRADES_ALLOWED:
                if (zCarPhysicsData.HaveAnyUpgradesBeenFitted)
                {
                    return false;
                }

                return true;
            case eRaceEventRestrictionType.CAR_NO_NITROUS_ALLOWED:
                if (zCarPhysicsData.HasAnyNitrousUpgradeBeenFitted)
                {
                    return false;
                }

                return true;
            case eRaceEventRestrictionType.CAR_NO_TURBO_ALLOWED:
                if (zCarPhysicsData.HasAnyTurboUpgradeBeenFitted)
                {
                    return false;
                }

                return true;
            case eRaceEventRestrictionType.CAR_NO_TYRES_ALLOWED:
                if (zCarPhysicsData.HasAnyTyreUpgradeBeenFitted)
                {
                    return false;
                }

                return true;
            case eRaceEventRestrictionType.CAR_NITROUS_NEEDED:
                if (!zCarPhysicsData.HasAnyNitrousUpgradeBeenFitted)
                {
                    return false;
                }

                return true;
            case eRaceEventRestrictionType.CAR_TYRES_NEEDED:
                if (!zCarPhysicsData.HasAnyTyreUpgradeBeenFitted)
                {
                    return false;
                }

                return true;
            case eRaceEventRestrictionType.CASH_MORE_THAN:
                if (PlayerProfileManager.Instance.ActiveProfile.GetCurrentCash() < this.Cash)
                {
                    return false;
                }

                return true;
            case eRaceEventRestrictionType.CASH_LESS_THAN:
                if (PlayerProfileManager.Instance.ActiveProfile.GetCurrentCash() > this.Cash)
                {
                    return false;
                }

                return true;
            case eRaceEventRestrictionType.CAR_PP_MORE_THAN:
                if (newPerformanceIndex <= (int) this.Horsepower)
                {
                    return false;
                }

                return true;
            case eRaceEventRestrictionType.CAR_PP_LESS_THAN:
                if (newPerformanceIndex >= (int) this.Horsepower)
                {
                    return false;
                }

                return true;
            case eRaceEventRestrictionType.CAR_PRO:
                if (!PlayerProfileManager.Instance.ActiveProfile.IsProCarOwned(zCarPhysicsData.carInfo.Key))
                {
                    return false;
                }

                return true;
        }

        return true;
    }

    public string GetTextureName()
	{
		switch (this.RestrictionType)
		{
		case eRaceEventRestrictionType.CAR_MANUFACTURER:
		case eRaceEventRestrictionType.CAR_CLASS:
		case eRaceEventRestrictionType.CAR_MODEL:
		case eRaceEventRestrictionType.CAR_PRO:
			return "map_restriction_manufacturer";
		case eRaceEventRestrictionType.CAR_WEIGHT_MORE_THAN:
			return "map_restriction_weight_up";
		case eRaceEventRestrictionType.CAR_WEIGHT_LESS_THAN:
			return "map_restriction_weight_down";
		case eRaceEventRestrictionType.CAR_HORSEPOWER_LESS_THAN:
			return "map_restriction_hp";
		case eRaceEventRestrictionType.CAR_DRIVE_WHEELS:
			if (this.DriveWheels == eDriveType.FWD)
			{
				return "map_restriction_fwd";
			}
			if (this.DriveWheels == eDriveType.RWD)
			{
				return "map_restriction_rwd";
			}
			return "map_restriction_4wd";
		case eRaceEventRestrictionType.CAR_NO_UPGRADES_ALLOWED:
			return "map_restriction_none";
		case eRaceEventRestrictionType.CAR_NO_NITROUS_ALLOWED:
			return "map_restriction_nos_no";
		case eRaceEventRestrictionType.CAR_NO_TURBO_ALLOWED:
			return "map_restriction_turbo_no";
		case eRaceEventRestrictionType.CAR_NO_TYRES_ALLOWED:
			return "map_restriction_tires_no";
		case eRaceEventRestrictionType.CAR_NITROUS_NEEDED:
			return "map_restriction_nos";
		case eRaceEventRestrictionType.CAR_TYRES_NEEDED:
			return "map_restriction_tires";
		case eRaceEventRestrictionType.CASH_MORE_THAN:
		case eRaceEventRestrictionType.CASH_LESS_THAN:
			return "map_restriction_cash";
		case eRaceEventRestrictionType.CAR_PP_MORE_THAN:
		case eRaceEventRestrictionType.CAR_PP_LESS_THAN:
			return "map_restriction_pp";
		}
		return string.Empty;
	}
}
