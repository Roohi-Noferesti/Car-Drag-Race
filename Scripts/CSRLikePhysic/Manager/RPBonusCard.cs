using System;
using I2.Loc;

public class RPBonusCard : RPBonus
{
	public RewardSize RewardSize;

	public RPBonusWindow BonusWindow;

	public override float Multiplier
	{
		get
		{
			return RPBonusManager.GetRPCardReward(this.RewardSize).Reward;
		}
	}

	public override bool AwardThisBonus()
	{
		if (!this.BonusWindow.IsInvalid())
		{
			return ServerSynchronisedTime.Instance.ServerTimeValid && this.BonusWindow.IsInsideWindow(ServerSynchronisedTime.Instance.GetDateTime());
		}
		if (ServerSynchronisedTime.Instance.ServerTimeValid)
		{
			this.StartBonusCardTimeNow();
			return true;
		}
		return true;
	}

	public override RP_BONUS_TYPE GetBoostType()
	{
		return RP_BONUS_TYPE.CARD_BONUS;
	}

	public override string GetBoostTypeText()
	{
		return LocalizationManager.GetTranslation("TEXT_RPBOOST_CARDBONUS");
	}

	public override string GetBoostReason()
	{
		return (this.BonusWindow != null) ? this.BonusWindow.GetTimeRemainingString() : string.Empty;
	}

	public override RPBonusWindow GetBonusWindow()
	{
		return this.BonusWindow;
	}

	public override void Populate(RPMultiplierBonus inBonus)
	{
		this.BonusWindow = new RPBonusWindow();
		base.Populate(inBonus);
	}

	private void StartBonusCardTimeNow()
	{
		int duration = RPBonusManager.GetRPCardReward(this.RewardSize).Duration;
		DateTime dateTime = ServerSynchronisedTime.Instance.GetDateTime();
		DateTime inFinish = dateTime.AddMinutes((double)duration);
		this.BonusWindow = new RPBonusWindow(dateTime, inFinish);
	}
}
