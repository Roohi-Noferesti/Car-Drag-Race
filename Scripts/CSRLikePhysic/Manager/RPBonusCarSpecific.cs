using System;
using System.Globalization;
using I2.Loc;

public class RPBonusCarSpecific : RPBonus
{
	private RPBonusWindow bonusWindow;

	public override float Multiplier
	{
		get
		{
			return this.MultiplierBonus.Bonus;
		}
	}

	public override bool AwardThisBonus()
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		bool flag = string.IsNullOrEmpty(this.MultiplierBonus.CarDBKey) || this.MultiplierBonus.CarDBKey == activeProfile.CurrentlySelectedCarDBKey;
		return flag && this.bonusWindow.IsAbleToAwardBonus();
	}

	public override RP_BONUS_TYPE GetBoostType()
	{
		return (!string.IsNullOrEmpty(this.MultiplierBonus.CarDBKey) && this.bonusWindow.IsAbleToAwardBonus()) ? RP_BONUS_TYPE.CAR_SPECIFIC_BONUS : RP_BONUS_TYPE.RP_BONUS_EVENT;
	}

	public override string GetBoostTypeText()
	{
		string textID = (this.GetBoostType() != RP_BONUS_TYPE.RP_BONUS_EVENT) ? "TEXT_RPBOOST_CARSPECIFICBONUS" : ((!this.AwardThisBonus()) ? "TEXT_RPBOOST_UPCOMING_WEEKENDBONUS" : "TEXT_RPBOOST_WEEKENDBONUS");
		return LocalizationManager.GetTranslation(textID);
	}

	public override string GetBoostReason()
	{
		string result = string.Empty;
		if (!this.bonusWindow.IsAbleToAwardBonus())
		{
			TimeSpan timeSpan = this.bonusWindow.TimeUntilStart();
			if (timeSpan.Days > 1)
			{
				result = string.Format(LocalizationManager.GetTranslation("TEXT_SEASON_DAYS_REMAINING_WT_THEME"), timeSpan.Days);
			}
			else
			{
				result = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours + timeSpan.Days * 24, timeSpan.Minutes, timeSpan.Seconds);
			}
		}
		else if (this.GetBoostType() == RP_BONUS_TYPE.CAR_SPECIFIC_BONUS && CarDatabase.Instance != null)
		{
			CarInfo carOrNull = CarDatabase.Instance.GetCarOrNull(this.MultiplierBonus.CarDBKey);
			if (carOrNull != null)
			{
				result = LocalizationManager.GetTranslation(carOrNull.MediumName);
			}
			else
			{
				result = this.MultiplierBonus.CarDBKey;
			}
		}
		else
		{
			TimeSpan timeSpan2 = this.bonusWindow.TimeRemaining();
			if (timeSpan2.Days <= 1)
			{
				return string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan2.Hours + timeSpan2.Days * 24, timeSpan2.Minutes, timeSpan2.Seconds);
			}
		}
		return result;
	}

	public override void Populate(RPMultiplierBonus inBonus)
	{
		DateTime inStart = DateTime.MinValue;
		DateTime inFinish = DateTime.MaxValue;
		CultureInfo invariantCulture = CultureInfo.InvariantCulture;
		if (!string.IsNullOrEmpty(inBonus.StartTime))
		{
			inStart = DateTime.ParseExact(inBonus.StartTime, RPBonusConfiguration.DateFormat, invariantCulture);
		}
		if (!string.IsNullOrEmpty(inBonus.EndTime))
		{
			inFinish = DateTime.ParseExact(inBonus.EndTime, RPBonusConfiguration.DateFormat, invariantCulture);
		}
		this.bonusWindow = new RPBonusWindow(inStart, inFinish);
		base.Populate(inBonus);
	}

	public override RPBonusWindow GetBonusWindow()
	{
		return this.bonusWindow;
	}
}
