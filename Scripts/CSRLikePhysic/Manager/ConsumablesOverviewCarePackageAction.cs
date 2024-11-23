using System;
using KingKodeStudio;

public class ConsumablesOverviewCarePackageAction : CarePackageActionBase
{
	public override void Action(CarePackageLevel level)
	{
		level.CalculatedReward.GiveToPlayer();
		ScreenManager.Instance.PushScreenWithFakedHistory(ScreenID.ConsumableOverview, new ScreenID[]
		{
			ScreenID.MultiplayerModeSelect
		});
		level.Deactivate();
	}
}
