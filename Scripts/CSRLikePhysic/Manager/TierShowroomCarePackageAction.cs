using System;

public class TierShowroomCarePackageAction : CarePackageActionBase
{
	public override void Action(CarePackageLevel level)
	{
		level.CalculatedReward.GiveToPlayer();
		eCarTier highestUnlockedClass = RaceEventQuery.Instance.getHighestUnlockedClass();
		ShowroomScreen.ShowScreenWithTierCarList(highestUnlockedClass, false);
		level.Deactivate();
	}
}
