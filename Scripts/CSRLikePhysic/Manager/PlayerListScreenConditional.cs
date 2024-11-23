using System;

public class PlayerListScreenConditional : FlowConditionalBase
{
	public PlayerListScreenConditional()
	{
		GasTankIAPCondition gasTankIAPCondition = new GasTankIAPCondition();
		base.AddCondition(gasTankIAPCondition, 1, ConditionShowMode.AlwaysShow);
		base.AddCondition(new FreeGasPopupCondition(gasTankIAPCondition), 2, ConditionShowMode.ShowIfFirstCondition);
	}

	protected override bool IsConditionalActive()
	{
		return true;
	}
}
