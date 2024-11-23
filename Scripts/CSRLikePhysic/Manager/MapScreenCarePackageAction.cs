using System;
using KingKodeStudio;

public class MapScreenCarePackageAction : CarePackageActionBase
{
	public override void Action(CarePackageLevel level)
	{
		level.CalculatedReward.GiveToPlayer();
		ScreenManager.Instance.PushScreen(ScreenID.CareerModeMap);
		level.Deactivate();
	}
}
