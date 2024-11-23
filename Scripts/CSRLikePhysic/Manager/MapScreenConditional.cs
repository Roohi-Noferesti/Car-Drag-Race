using System;

public class MapScreenConditional : FlowConditionalBase
{
	public MapScreenConditional()
	{
        //base.AddCondition(new FriendsEasyRaceCondition(), 10, ConditionShowMode.AlwaysShow);
        //base.AddCondition(new FriendsInviteFlowCondition(), 20, ConditionShowMode.AlwaysShow);
        //base.AddCondition(new RenewTeamIAPConditional(), 5, ConditionShowMode.AlwaysShow);
        //base.AddCondition(new RenewUnlimitedFuelConditional(), 6, ConditionShowMode.AlwaysShow);
		GasTankIAPCondition gasTankIAPCondition = new GasTankIAPCondition();
        //base.AddCondition(gasTankIAPCondition, 4, ConditionShowMode.AlwaysShow);
		base.AddCondition(new DoDailyBattlePopupConditional(), 3, ConditionShowMode.ShowIfFirstCondition);
		base.AddCondition(new FreeGasPopupCondition(gasTankIAPCondition), 4, ConditionShowMode.ShowIfFirstCondition);
		base.AddCondition(new AllHardRacesTutorialConditional(), 30, ConditionShowMode.AlwaysShow);
	}

	protected override bool IsConditionalActive()
	{
		return true;
	}
}
