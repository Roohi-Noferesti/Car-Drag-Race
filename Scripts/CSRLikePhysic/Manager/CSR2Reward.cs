using System;
using I2.Loc;

[Serializable]
public class CSR2Reward
{
	public ERewardType rewardType;

	public string name = string.Empty;

	public eUpgradeType ePartType;

	public int partGrade;

	public bool matchOwnedCar;

	public string GetRewardTitleText(int quantity)
	{
		string text = string.Empty;
		switch (this.rewardType)
		{
		case ERewardType.Cash:
			text = "TEXT_CASH";
		        break;
		case ERewardType.Gold:
			text = "TEXT_GOLD";
            break;
		case ERewardType.TireSlicks:
			return "Tire slicks";
		case ERewardType.SpecialNitrous:
			text = "TEXT_SHOP_LIST_ITEM_SUPER_NITROUS";
            break;
		case ERewardType.GearService:
			return "Gear service";
		case ERewardType.RacingFuel:
			return "Fuel";
		case ERewardType.LockupKey:
			text = "TEXT_BRONZE_KEYS";
            break;
		case ERewardType.SilverGachaKey:
			text = "TEXT_SILVER_KEYS";
            break;
		case ERewardType.GoldGachaKey:
			text = "TEXT_GOLD_KEYS";
            break;
		case ERewardType.FuelPip:
			text = "TEXT_FUEL_PIPS";
            break;
		case ERewardType.Car:
			text = this.name;
            break;
		case ERewardType.EvoUpgrade:
			if (string.IsNullOrEmpty(this.name))
			{
				text = LocalizationManager.GetTranslation("TEXT_EVO_UPGRADE");
			}
			else
			{
				CarInfo car = CarDatabase.Instance.GetCar(this.name);
				if (car != null)
				{
					text = LocalizationManager.GetTranslation(car.ShortName) + " " + LocalizationManager.GetTranslation("TEXT_EVO_UPGRADE");
				}
			}
			return text;
		case ERewardType.FusionUpgrade:
			if (!string.IsNullOrEmpty(this.name))
			{
                //Manufacturer manufacturer = ManufacturerDatabase.Instance.Manufacturers[this.name];
                //if (manufacturer != null)
                //{
                //    text = LocalizationManager.GetTranslation(manufacturer.translatedName) + " ";
                //}
			}
			if (this.ePartType != eUpgradeType.INVALID)
			{
				text = text + this.ePartType.ToString() + " ";
			}
            //if (this.partGrade > 0)
            //{
            //    text = text + LocalizationManager.GetTranslation(FusingSlotButton.gradestrings[this.partGrade - 1]) + " ";
            //}
			return text + LocalizationManager.GetTranslation((quantity <= 1) ? "TEXT_FUSION_UPGRADE" : "TEXT_FUSION_UPGRADES");
		case ERewardType.Crate:
			return "Gacha Crate";
		case ERewardType.RP:
			text = "TEXT_PRIZE_RP";
            break;
		case ERewardType.FreeUpgrade:
			text = "TEXT_PRIZE_UPGRADE";
            break;
		case ERewardType.CrewRP:
			text = "TEXT_PRIZE_CREWRP";
            break;
		case ERewardType.WildcardToken:
			text = "TEXT_PRIZE_WILDCARDTOKEN";
            break;
		}
		return LocalizationManager.GetTranslation(text);
	}

	public override string ToString()
	{
		return string.Format("CSR2Reward:\nReward Type: {0}\nName: {1}\nPart Type: {2}\nPart Grade: {3}\n", new object[]
		{
			this.rewardType,
			this.name,
			this.ePartType,
			this.partGrade
		});
	}
}
