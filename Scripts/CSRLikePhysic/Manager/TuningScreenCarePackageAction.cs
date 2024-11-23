using System;
using KingKodeStudio;

public class TuningScreenCarePackageAction : CarePackageActionBase
{
	public override void Action(CarePackageLevel level)
	{
		level.CalculatedReward.GiveToPlayer();
		ScreenManager.Instance.PushScreen(ScreenID.Tuning);
		level.Deactivate();
	}
}
