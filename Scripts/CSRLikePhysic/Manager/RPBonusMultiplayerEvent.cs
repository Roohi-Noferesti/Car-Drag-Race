using System;
using I2.Loc;

public class RPBonusMultiplayerEvent : RPBonus
{
	private float multiplier;

	private RPBonusWindow bonusWindow;

	public override float Multiplier
	{
		get
		{
			return this.multiplier;
		}
	}

	public RPBonusMultiplayerEvent(float multiplier, RPBonusWindow bonusWindow)
	{
		this.multiplier = multiplier;
		this.bonusWindow = bonusWindow;
		this.Populate(null);
	}

	public override bool AwardThisBonus()
	{
		return ServerSynchronisedTime.Instance.ServerTimeValid && this.bonusWindow.IsInsideWindow(ServerSynchronisedTime.Instance.GetDateTime());
	}

	public override RP_BONUS_TYPE GetBoostType()
	{
		return RP_BONUS_TYPE.MULTIPLAYER_EVENT;
	}

	public override string GetBoostTypeText()
	{
		return LocalizationManager.GetTranslation("TEXT_RPBOOST_MULTIPLAYER_EVENT");
	}

	public override string GetBoostReason()
	{
		return (this.bonusWindow != null) ? this.bonusWindow.GetTimeRemainingString() : string.Empty;
	}

	public override RPBonusWindow GetBonusWindow()
	{
		return this.bonusWindow;
	}
}
