using System;
using System.Collections.Generic;

[Serializable]
public class ModeInfo
{
	public float CashEntryMultiplier = 1f;

	public float CashRewardMultiplier = 1f;

	public int GoldEntryFee;

	public float RPMultiplier = 1f;

	public float BossCarProbability = 0.1f;

	public string Title;

	public string Description;

	public List<RaceEventRestriction> Restrictions = new List<RaceEventRestriction>();

    public MultiplayerModeTheme Theme = new MultiplayerModeTheme();

	public void Initialise()
	{
		foreach (RaceEventRestriction current in this.Restrictions)
		{
			current.Initialise();
		}
	}

	public bool DoesMeetRestrictions(CarPhysicsSetupCreator zCarPhysicsSetup)
	{
		foreach (RaceEventRestriction current in this.Restrictions)
		{
			if (!current.DoesMeetRestriction(zCarPhysicsSetup))
			{
				return false;
			}
		}
		return true;
	}
}
