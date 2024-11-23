using System;
using I2.Loc;

public class RPBonusAd : RPBonus
{
	public RPBonusWindow BonusWindow;

	private float _multiplier;

	public override float Multiplier
	{
		get
		{
			return this._multiplier;
		}
	}

	public RPBonusAd(float multiplier)
	{
		this._multiplier = multiplier;
	}

	public override bool AwardThisBonus()
	{
		return this.BonusWindow != null && this.BonusWindow.IsAbleToAwardBonus();
	}

	public override RP_BONUS_TYPE GetBoostType()
	{
		return RP_BONUS_TYPE.VIDEOAD_BONUS;
	}

	public override string GetBoostTypeText()
	{
		return LocalizationManager.GetTranslation((!this.AwardThisBonus()) ? "TEXT_RPBOOST_VIDEOAD" : "TEXT_RPBOOST_VIDEOAD_ACTIVE");
	}

	public override string GetBoostReason()
	{
		if (this.BonusWindow == null || !this.BonusWindow.IsAbleToAwardBonus())
		{
			return LocalizationManager.GetTranslation("TEXT_RPBOOST_VIDEOAD");
		}
		TimeSpan timeSpan = this.BonusWindow.TimeRemaining();
		return string.Format(LocalizationManager.GetTranslation("TEXT_UNITS_TIME_MINUTES_AND_SECONDS"), (int)timeSpan.TotalMinutes, timeSpan.Seconds);
	}
}
