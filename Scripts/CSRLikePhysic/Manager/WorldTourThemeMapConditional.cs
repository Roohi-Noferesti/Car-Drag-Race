using System;

public class WorldTourThemeMapConditional : FlowConditionalBase
{
	public WorldTourThemeMapConditional()
	{
		base.AddCondition(new WorldTourThemeFirstTimeCondition(), 10, ConditionShowMode.AlwaysShow);
		base.AddCondition(new WorldTourThemeSecondTimeCondition(), 20, ConditionShowMode.AlwaysShow);
	}

	protected override bool IsConditionalActive()
	{
		return false;
	}
}
