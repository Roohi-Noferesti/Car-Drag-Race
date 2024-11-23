using System;
using I2.Loc;

public class RPBonusChain : RPBonus
{
	public override float Multiplier
	{
		get
		{
			return StreakManager.Chain.Multiplier;
		}
	}

	public override bool AwardThisBonus()
	{
		return StreakManager.Chain.Active();
	}

	public override RP_BONUS_TYPE GetBoostType()
	{
		return RP_BONUS_TYPE.STREAK_BONUS;
	}

	public override string GetBoostTypeText()
	{
		return LocalizationManager.GetTranslation("TEXT_RPBOOST_STREAKBONUS");
	}

	public override string GetBoostReason()
	{
		return string.Empty;
	}
}
