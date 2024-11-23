using System;
using KingKodeStudio;

public class MultiplayerPaneCarePackageAction : CarePackageActionBase
{
	public override void Action(CarePackageLevel level)
	{
		level.CalculatedReward.GiveToPlayer();
		ScreenManager.Instance.PushScreen(ScreenID.MultiplayerModeSelect);
		level.Deactivate();
	}
}
