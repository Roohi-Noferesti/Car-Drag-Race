using System;
using System.Collections.Generic;
using I2.Loc;

public class RPBonusCars : RPBonus
{
	private List<float> perCarMultipliers;

	private List<int> perNumCars;

	private float BoostPerCar
	{
		get
		{
			return this.MultiplierBonus.Bonus;
		}
	}

	public override float Multiplier
	{
		get
		{
			if (this.perNumCars.Count != this.perCarMultipliers.Count)
			{
				return 0f;
			}
			PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
			int num = activeProfile.CarsOwned.Count + ArrivalManager.Instance.HowManyCarsAreOnOrder();
			float num2 = 0f;
			for (int i = 0; i < this.perNumCars.Count; i++)
			{
				if (num < this.perNumCars[i])
				{
					num2 += (float)num * this.perCarMultipliers[i];
					num = 0;
					break;
				}
				num2 += (float)this.perNumCars[i] * this.perCarMultipliers[i];
				num -= this.perNumCars[i];
			}
			if (num > 0)
			{
				num2 += (float)num * this.BoostPerCar;
			}
			return num2;
		}
	}

	public override bool AwardThisBonus()
	{
		return true;
	}

	public override RP_BONUS_TYPE GetBoostType()
	{
		return RP_BONUS_TYPE.CAR_BONUS;
	}

	public override string GetBoostTypeText()
	{
		return LocalizationManager.GetTranslation("TEXT_RPBOOST_CARSBONUS");
	}

	public override string GetBoostReason()
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		int num = activeProfile.CarsOwned.Count + ArrivalManager.Instance.HowManyCarsAreOnOrder();
		return string.Format(LocalizationManager.GetTranslation("TEXT_RPBOOST_CARSREASON"), num);
	}

	public override void Populate(RPMultiplierBonus inBonus)
	{
		this.perCarMultipliers = inBonus.PerCarBonuses;
		this.perNumCars = inBonus.CumulativeNumCars;
		base.Populate(inBonus);
	}
}
