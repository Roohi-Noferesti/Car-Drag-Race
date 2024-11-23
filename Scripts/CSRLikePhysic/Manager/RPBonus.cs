using System;

public abstract class RPBonus
{
	public RPMultiplierBonus MultiplierBonus;

	public abstract float Multiplier
	{
		get;
	}

	public abstract bool AwardThisBonus();

	public abstract RP_BONUS_TYPE GetBoostType();

	public abstract string GetBoostTypeText();

	public abstract string GetBoostReason();

	public virtual void Populate(RPMultiplierBonus inBonus)
	{
		this.MultiplierBonus = inBonus;
	}

	public virtual RPBonusWindow GetBonusWindow()
	{
		return null;
	}
}
