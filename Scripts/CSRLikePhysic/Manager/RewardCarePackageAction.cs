using System;

public class RewardCarePackageAction : CarePackageActionBase
{
	public override void Action(CarePackageLevel level)
	{
		level.CalculatedReward.GiveToPlayer();
		level.Deactivate();
	}
}
